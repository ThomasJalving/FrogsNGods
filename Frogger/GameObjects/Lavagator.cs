using Microsoft.Xna.Framework;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement.LevelGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger.GameObjects
{
    /// <summary>
    /// A SpecialGameObject with some extra properties
    /// </summary>
    class Lavagator : LaneObject
    {
        private bool semiFake = false;

        public Lavagator(string[] objectstats, float speedMod, Vector2 Position)
            :base(objectstats, speedMod, Position)
        {
            string baseSpriteName = objectstats[0].Split(' ')[0];
            string size = objectstats[0].Split(' ')[1];
            string fakeLoopName = baseSpriteName + "_Fake " + size;
            string transitionFakeName = baseSpriteName + "_Transition_Fake " + size;
            string transitionRealName = baseSpriteName + "_Transition " + size;
            LoadAnimation("Fake", fakeLoopName, 9, 0.1f, toRight);
            LoadAnimation("TransitionToFake", transitionFakeName, 9, 0.1f, toRight);
            LoadAnimation("TransitionToReal", transitionRealName, 9, 0.1f, toRight);
            LoadAnimation("Normal", objectstats[0], 9, 0.1f, toRight);
            StartAnimation("Normal", 1f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!animation.Animating)
            {
                switch (currentAnimationName)
                {
                    case "Fake":
                        StartAnimation("TransitionToReal", 0.9f);
                        break;
                    case "Normal":
                        StartAnimation("TransitionToFake", 0.9f);
                        break;
                    case "TransitionToReal":
                        StartAnimation("Normal", 4.5f);
                        semiFake = false;
                        break;
                    case "TransitionToFake":
                        StartAnimation("Fake", 4.5f);
                        semiFake = true;
                        break;
                }
            }
        }

        public override Rectangle BoundingBox
        {
            get
            {
                if (!semiFake)
                {
                    return base.BoundingBox;
                }

                if (toRight)
                {
                    return new Rectangle((int)Position.X - (int)animation.Origin.X, (int)Position.Y - (int)animation.Origin.Y, animation.Width - Level.tileHeight, animation.Height);
                }
                else
                {
                    return new Rectangle((int)Position.X - (int)animation.Origin.X + Level.tileHeight, (int)Position.Y - (int)animation.Origin.Y, animation.Width - Level.tileHeight, animation.Height);
                }
            }
        }
    }
}
