using Microsoft.Xna.Framework;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement.LevelGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.GameObjects
{
    /// <summary>
    /// A SpecialGameObject with some extra properties
    /// </summary>
    class DuskFlower : SpecialGameObject
    {
        AnimatedGameObject Flower; //A seperate object is drawn on top to be able to use 2 different animations at the same time
        float pollenTimer;

        public DuskFlower(bool pickupable, string spritename, Vector2 Position)
            : base(pickupable, spritename, Position)
        {
            LoadAnimation("expand", spritename + "_Pollen", 13, 0.3f);
            animations["expand"].Origin = new Vector2(Level.tileWidth, Level.tileWidth);
            StartAnimation("expand", 3.9f);

            Flower = new AnimatedGameObject(spritename);
            Flower.Position = Position;
            Flower.LoadAnimation("AnimationLoop", spritename, 13, 0.1f, looping:true);
            objectType = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Flower.Update(gameTime);
            pollenTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (pollenTimer >= 5.5)
            {
                StartAnimation("expand", 3.9f);
                objectType = "Duskflower";
                pollenTimer = 0;
            }
            if (!animation.Animating)
                objectType = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            Flower.Draw(gameTime, spriteBatch);
        }
    }
}
