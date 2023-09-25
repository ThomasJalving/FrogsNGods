using Frogger.GameObjects;
using Frogger.GameManagement.LevelGenerator;
using Microsoft.Xna.Framework;

namespace Frogger.GameManagement
{
    /// <summary>
    /// The class that follows a target and changes the drawing position of everything
    /// </summary>
    class Camera
    {
        const int tileOffset = 4;
        private int position, screenOffset, offsetX;
        private float rotation = 0, scale = 1;
        private Player target;

        public Camera(int offsetX = 0)
        {
            screenOffset = GameEnvironment.Screen.Y;
            this.offsetX = offsetX;
        }
        
        /// <summary>
        /// Moves the camera to follow the player, with specific restraints to how far
        /// </summary>
        public void Follow()
        {
            if (target != null)
            {
                if (target.Position.Y > target.Level.activeChunks[0].Position.Y - Level.tileHeight * tileOffset)
                {//keeps the camera completely static if you're too close to the bottom edge of the screen
                    position = (int)target.Level.activeChunks[0].Position.Y - screenOffset + Level.tileHeight;
                }
                else if (target.Position.Y > target.Level.activeChunks[0].Position.Y - Level.tileHeight * (tileOffset + 1))
                {//moves the camera with the player in the transition between being static and actually following the player
                    position = (int)target.Level.activeChunks[0].Position.Y - screenOffset + Level.tileHeight + ((int)target.Position.Y % 32);
                }
                else
                {//actually moves the camera with the player
                    position = (int)target.Position.Y - screenOffset + (Level.tileHeight * (tileOffset + 1));
                }
            }
            else
            {
                position = 0;
            }
        }

        public void Reset()
        {
            position = 0;
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(offsetX, -position, 0) *
                    Matrix.CreateScale(scale, scale, 1) *
                    Matrix.CreateRotationZ(rotation);
            }
        }

        public Player Target
        {
            set { target = value; }
        }
    }
}
