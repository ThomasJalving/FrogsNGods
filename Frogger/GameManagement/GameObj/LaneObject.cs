using Microsoft.Xna.Framework;
using Frogger.GameManagement.LevelGenerator;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// The class used for GameObjects that spawn and move in a horizontal lane
    /// A LaneObject can either carry or kill the player
    /// </summary>
    public class LaneObject : AnimatedGameObject
    {
        protected bool toRight, deadly, fake;

        public LaneObject(string[] objectstats, float speedMod, Vector2 Position, bool Fake = false) :
            base(objectstats[0])
        {
            deadly = bool.Parse(objectstats[2]);
            toRight = bool.Parse(objectstats[3]);
            float Speed = int.Parse(objectstats[1]) * speedMod;
            fake = Fake;

            //as the sprite names contain their size in the amount of tiles we just split that part of the name to get the right sizes
            string size = objectstats[0].Split(' ')[1];
            int width = int.Parse(size.Split('x')[0]);
            int height = int.Parse(size.Split('x')[1]);
            int frames = AssetManager.GetSprite(objectstats[0]).Height/(Level.tileHeight*height);

            LoadAnimation("animationLoop", objectstats[0], frames, 0.1f, toRight, true);
            StartAnimation("animationLoop");
            
            //based on the direction it goes it gets moved barely outside of the screen
            if (toRight)
            {
                position = new Vector2(Position.X - BoundingBox.Width, Position.Y);
                mirrored = true;
            }
            else
            {
                position = new Vector2(Position.X, Position.Y);
                Speed *= -1;
                mirrored = false;
            }
            velocity = new Vector2(Speed, 0);
        }

        //constructor for the stationary platforms
        public LaneObject( Vector2 Position,string SpriteName) :
            base(SpriteName)
        {
            deadly = false;
            position = Position;
            velocity = Vector2.Zero;

            string size = SpriteName.Split(' ')[1];
            int width = int.Parse(size.Split('x')[0]);
            int height = int.Parse(size.Split('x')[1]);
            int frames = AssetManager.GetSprite(SpriteName).Height / (32 * height);

            LoadAnimation("animationLoop", SpriteName, frames, 0.5f, false, true);
            StartAnimation("animationLoop");
        }

        public bool Deadly
        {
            get { return deadly; }
        }

        public bool Fake
        {
            get { return fake; }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(position.X < -BoundingBox.Width || position.X > Level.tileWidth * Level.ChunkWidth)
            {
                deletable = true;
            }
        }
    }
}