using Microsoft.Xna.Framework;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement;
using System.Globalization;
using System;

namespace Frogger.GameObjects
{
    /// <summary>
    /// Class that handles all of the creation of LaneObjects that transition between fake and real
    /// </summary>
    class TurtleSpawner : Spawner
    {
        protected bool dangerousgroup;

        public TurtleSpawner(int vertpos, bool Deadly, bool ToRight, string StartCooldown, Chunk chunk, string[] spawnerstats)
            : base(vertpos, Deadly, ToRight, StartCooldown, chunk, spawnerstats)
        {
            if (GameEnvironment.Random.Next(100) < dangerchance)
            {
                dangerousgroup = true;
            }
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

        /// <summary>
        /// Checks the overall cooldown and the burstcooldown to see if it should start a spawning batch, or spawn a single LaneObject
        /// </summary>
        protected override void CheckRegularTimers()
        {
            //the delays between entire groups of LaneObjects
            if (regulartimer >= cooldown && !starting)
            {
                //randomly generate whether the next group is going to be special, and prevent it from doing it twice in a row
                if (GameEnvironment.Random.Next(100) < dangerchance && !dangerousgroup)
                {
                    dangerousgroup = true;
                }
                else
                {
                    dangerousgroup = false;
                }
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

        protected override void Burstspawn()
        {
            if (dangerousgroup)
            {
                chunk.laneObjects.Add(new Turtle(objectStats, speedMod, position, burstcooldown*(currentburst-1)));
            }
            else
            {
                chunk.laneObjects.Add(new LaneObject(objectStats, speedMod, position));
            }

            currentburst -= 1;
            if (currentburst == 0)
            {
                bursting = false;
            }
        }
    }
}
