using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// A class that loads animations into a Dictionary and draws the current animation.
    /// the rest of its features are inherited
    /// </summary>
    public class AnimatedGameObject : SpriteSheetGameObject
    { 
        public Animation animation { get; protected set; }
        public Dictionary<string, Animation> animations;
        protected string currentAnimationName;

        public AnimatedGameObject(string spriteName =null, int variation = 0)
            :base(spriteName, variation, 1)
        {
            animations = new Dictionary<string, Animation>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            animation.Update(gameTime);
        }

        /// <summary>
        /// Loads a new animation and adds it to the animations Dictionary.
        /// Also gives the animation an id string.
        /// </summary>
        public virtual void LoadAnimation(string id, string assetName, int frames, float frameTime = 0.1f, bool mirrored = false , bool looping = false)
        {
            if (assetName != null)
            {
                animations.Add(id, new Animation(AssetManager.GetSprite(assetName),frames, frameTime, mirrored, looping));
                if(animation==null)
                    StartAnimation(id);
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
                animation.Draw(spriteBatch, gameTime, position);
        }

        /// <summary>
        /// Switches to the requested animation and keeps it playing for a certain amount of seconds.
        /// </summary>
        public void StartAnimation(string id, float seconds = 0)
        {
            if(animation!=null)
                animation.Stop();

            currentAnimationName = id;
            animation = animations[id];
            animation.Animate(seconds);
        }

        public override Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X-(int)animation.Origin.X, (int)Position.Y-(int)animation.Origin.Y, animation.Width, animation.Height); }
        }

        /// <summary>
        /// Returns the current animation.
        /// </summary>
        public Animation CurrentAnimation
        {
            get { return animation; }
        }

        public string CurrentAnimationName
        {
            get { return currentAnimationName; }
        }


    }
}
