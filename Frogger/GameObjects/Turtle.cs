using Microsoft.Xna.Framework;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger.GameObjects
{
    /// <summary>
    /// A specific version of the LaneObject class that transitions between a fake and real platform
    /// </summary>
    class Turtle : LaneObject
    {
        public Turtle(string[] objectstats,float speedMod, Vector2 Position, double animationDelay)
            :base(objectstats, speedMod, Position)
        {
            string baseSpriteName = objectstats[0].Split(' ')[0];
            string size = objectstats[0].Split(' ')[1];
            string fakeLoopName = baseSpriteName + "_Fake " + size;
            string transitionFakeName = baseSpriteName + "_Transition_Fake " + size;
            string transitionRealName = baseSpriteName + "_Transition " + size;
            LoadAnimation("Fake", fakeLoopName, 10/int.Parse(size.Split('x')[1]), 0.1f, toRight);
            LoadAnimation("TransitionToFake", transitionFakeName, 10 / int.Parse(size.Split('x')[1]), 0.2f, toRight);
            LoadAnimation("TransitionToReal", transitionRealName, 10 / int.Parse(size.Split('x')[1]), 0.2f, toRight);
            LoadAnimation("Normal", objectstats[0], 10 / int.Parse(size.Split('x')[1]), 0.1f, toRight);
            StartAnimation("Normal", 1f + (float)animationDelay);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(!animation.Animating)
            {
                switch (currentAnimationName)
                {
                    case "Fake":
                        StartAnimation("TransitionToReal", 1f);
                        break;
                    case "Normal":
                        StartAnimation("TransitionToFake", 1f);
                        break;
                    case "TransitionToReal":
                        StartAnimation("Normal", 3f);
                        fake = false;
                        break;
                    case "TransitionToFake":
                        StartAnimation("Fake", 2f);
                        fake = true;
                        break;
                }
            }
        }
    }
}