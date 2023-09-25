using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// Used for normal sprites
    /// </summary>
    public class SpriteGameObject : GameObject
    {
        protected Texture2D sprite;
        public Texture2D Sprite { get { return sprite; } private set { sprite = value; } }
        protected Rectangle rec;
        protected bool mirrored;

        public SpriteGameObject(string spriteName = "")
        {
            sprite = AssetManager.GetSprite(spriteName);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!visible)
                return;
            if(mirrored)
                spriteBatch.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height), null, Color.White, 0.0f, new Vector2(1, 1), SpriteEffects.FlipHorizontally, 0);
            else
                spriteBatch.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height), null, Color.White, 0.0f, new Vector2(1, 1), SpriteEffects.None, 0);           
        }

        public override Rectangle BoundingBox
        {
            get
            {              
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }
    }
}
