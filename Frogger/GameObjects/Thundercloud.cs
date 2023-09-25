using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement;
using Frogger.States;

namespace Frogger.GameObjects
{
    /// <summary>
    /// A SpecialGameObject with some extra properties
    /// </summary>
    class Thundercloud : SpecialGameObject
    {
        public Thundercloud(bool pickupable, string spritename, Vector2 position)
            : base(pickupable, spritename, position)
        {
            Pickupable = pickupable;
            objectType = spritename.Split('/')[1];
            this.position = position;

            LoadAnimation("lightning", spritename, 11, 2.0f / 11);
            StartAnimation("lightning");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Causes lightning to appear at random intervals; The frequency scales with difficulty
            if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying gamePlaying)
            {
                if (GameEnvironment.Random.Next((int)(200f * (((10f - gamePlaying.Level.activeGod.GetModifier(God.ModifierType.burstcooldown)) / 5f) + (0.1f * (5 - MathHelper.Clamp(10f - gamePlaying.Level.activeGod.GetModifier(God.ModifierType.burstcooldown), 0f, 5f)))))) == 0 && CurrentAnimation.Step == 0)
                    StartAnimation("lightning", 2.0f);
            }
            Effect();
        }
    }
}
