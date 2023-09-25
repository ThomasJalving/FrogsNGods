using Microsoft.Xna.Framework;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement;
using System;
using System.Globalization;

namespace Frogger.GameObjects
{
    /// <summary>
    /// Class that handles most of the creation of LaneObjects based on timers
    /// </summary>
    class Spawner : GameObject
    {
        protected int burstsize, currentburst;
        protected double baseCooldown, baseStartCooldown, baseBurstcooldown, baseDangerchance;
        protected double burstcooldown, cooldown, dangerchance;
        protected float regulartimer, bursttimer, speedMod;
        protected bool bursting, toRight, deadly, starting, spawnedDanger;
        protected string[] objectStats = new string[4];
        protected Chunk chunk;

        public Spawner(int vertpos, bool Deadly, bool ToRight, string StartCooldown, Chunk Chunk, string[] spawnerstats)
        {
            //order of info in the spawnerstats array in order: sprite, speed, cooldown, burstcooldown, burstsize, dangerchance
            //assign all the required info to get a working spawner
            chunk = Chunk;
            deadly = Deadly;
            toRight = ToRight;
            baseCooldown = double.Parse(spawnerstats[2],CultureInfo.InvariantCulture);
            baseBurstcooldown = double.Parse(spawnerstats[3], CultureInfo.InvariantCulture);
            baseStartCooldown = double.Parse(StartCooldown, CultureInfo.InvariantCulture);
            burstsize = int.Parse(spawnerstats[4]);
            currentburst = burstsize;
            baseDangerchance = int.Parse(spawnerstats[5], CultureInfo.InvariantCulture);

            if (ToRight)
            {
                position = new Vector2(0, vertpos);
            }
            else
            {
                position = new Vector2(Level.ChunkWidth * Level.tileWidth, vertpos);
            }
            //store some extra info in a new array to pass on to the spawned objects
            objectStats[0] = spawnerstats[0];
            objectStats[1] = spawnerstats[1];
            objectStats[2] = deadly.ToString();
            objectStats[3] = toRight.ToString();
            
            starting = true;
        }

        public override void Update(GameTime gameTime)
        {
            //constantly count up the timer to know when the next objects should be spawned
            regulartimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            bursttimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //the waiting time before the first spawn starts
            if (regulartimer >= baseStartCooldown && starting)
            {
                bursting = true;
                bursttimer = 0;
                starting = false;
                regulartimer = 0;
            }
            
            CheckRegularTimers();
        }
        
        /// <summary>
        /// Checks the overall cooldown and the burstcooldown to see if it should start a spawning batch, or spawn a single LaneObject
        /// </summary>
        protected virtual void CheckRegularTimers()
        {
            //the delays between entire groups of LaneObjects
            if (regulartimer >= cooldown && !starting)
            {
                bursting = true;
                currentburst = burstsize;
                bursttimer = 0;
                regulartimer = 0;
            }
            //delay the spawns inbetween individual obstacles or platforms in a group, it spawns one immediatly if it is the first in the group
            if (bursting && (bursttimer >= burstcooldown || burstsize == currentburst))
            {
                Burstspawn();
                bursttimer = 0;
            }
        }

        /// <summary>
        /// spawns the actual LaneObjects and adds them to the list of LaneObjects in the chunk
        /// </summary>
        protected virtual void Burstspawn()
        {
            string[] givenStats = new string[4];
            bool dangerous = false;
            for(int i = 0; i< objectStats.Length; i++)
            {
                givenStats[i] = objectStats[i];
            }
            //check if it can spawn a dangerous platform
            if (!deadly && !spawnedDanger)
            {
                //if it can, try to randomly generate one
                if (GameEnvironment.Random.Next(100) < dangerchance)
                {
                    if(!givenStats[0].Contains("Volcanic/Platform 3x1"))
                        givenStats[0] = objectStats[0].Split(' ')[0] + "_Fake " + objectStats[0].Split(' ')[1];
                    spawnedDanger = true;
                    dangerous = true;
                }
            }
            //specific versions of LaneObjects get a specific constructor
            if (givenStats[0].Contains("Sky/Platform"))
            {
                chunk.laneObjects.Add(new SkyRay(givenStats, speedMod, position));
            }
            else if (givenStats[0].Contains("Volcanic/Platform 3x1") && dangerous)
            {
                chunk.laneObjects.Add(new Lavagator(givenStats, speedMod, position));
            }
            else
            {
                chunk.laneObjects.Add(new LaneObject(givenStats, speedMod, position, dangerous));
            }
            
            currentburst -= 1;
            if (currentburst == 0)
            {
                spawnedDanger = false;
                bursting = false;
            }
        }

        /// <summary>
        /// used to get the modifiers from the gods and applying them to the spawner
        /// </summary>
        public void SetModifiers(God god)
        {
            float cooldownMod = ((10f - god.GetModifier(God.ModifierType.cooldown)) / 5f) + (0.1f * (5 - MathHelper.Clamp(10f - god.GetModifier(God.ModifierType.cooldown), 0f, 5f)));
            float burstcooldownMod = ((10f - god.GetModifier(God.ModifierType.burstcooldown)) / 5f) + (0.1f * (5 - MathHelper.Clamp(10f - god.GetModifier(God.ModifierType.burstcooldown), 0f, 5f)));
            float dangerchanceMod = (god.GetModifier(God.ModifierType.dangerChance) / 5f) + (0.1f * (5 - MathHelper.Clamp(god.GetModifier(God.ModifierType.dangerChance), 1f, 5f)));
            speedMod = (god.GetModifier(God.ModifierType.speed) / 5f) + (0.1f * (5 - MathHelper.Clamp(god.GetModifier(God.ModifierType.speed), 1f, 5f)));

            cooldown = baseCooldown * cooldownMod;
            dangerchance = baseDangerchance * dangerchanceMod;
            burstcooldown = baseBurstcooldown * burstcooldownMod;
        }
    }
}
