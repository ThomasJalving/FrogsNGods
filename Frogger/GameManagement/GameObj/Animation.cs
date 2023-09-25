using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// Contains one animation and inherits from spritesheet.
    /// can change the current frame according to time.
    /// </summary>
    public class Animation : SpriteSheet
    {
        protected bool animating, looping;
        protected float timer, frameLength, frameTimer;

        public Animation(Texture2D animation, int frames=10, float frameTime = 0.1f, bool mirrored = false,  bool looping = false)
            :base(animation, 0, variations: 1)
        {
            sheetColumns = 1;
            sheetRows = frames;
            frameLength = frameTime;
            mirror = mirrored;
            this.looping = looping;
        }

        /// <summary>
        /// Updates timers and moves to the next frame if neccesary.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (animating)
            {
                    while (frameTimer <= 0 )
                    {
                        frameTimer += frameLength;
                        if (step < sheetRows-1)
                            step++;
                        else
                            step = 0;
                    }

                    timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    frameTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (timer <= 0 && !looping)
                {
                    Stop();
                }
            }
            this.Recalculate();
        }
        
        /// <summary>
        /// Cancels the animation playback and returns to the first frame.
        /// </summary>
        public void Stop()
        {
            animating = false;
            step = 0;
        }

        /// <summary>
        /// Starts to play the animation for a certain timeframe.
        /// </summary>
        /// <param name="seconds"></param>
        public void Animate(float seconds)
        {
            animating = true;
            timer = seconds;
            frameTimer = frameLength;
        }

        public bool Animating
        {
            get { return animating; }
        }

        public int Step
        {
            get { return step; }
        }
    }
}
