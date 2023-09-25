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
    class SkyRay : LaneObject
    {
        /// <summary>
        /// A specific version of the LaneObject class where you can only collide with half of the body
        /// </summary>
        /// <param name="objectstats"></param>
        /// <param name="Position"></param>
        public SkyRay(string[] objectstats, float speedMod, Vector2 Position)
            : base(objectstats, speedMod, Position)
        {
        }

        public override Rectangle BoundingBox
        {
            get
            {
                if (toRight)
                {
                    return new Rectangle((int)Position.X - (int)animation.Origin.X + animation.Width/2, (int)Position.Y - (int)animation.Origin.Y, animation.Width / 2, animation.Height);
                }
                else
                {
                    return new Rectangle((int)Position.X - (int)animation.Origin.X, (int)Position.Y - (int)animation.Origin.Y, animation.Width / 2, animation.Height);
                }
            }
        }
    }
}
