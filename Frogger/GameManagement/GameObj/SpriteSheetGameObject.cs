using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// Inherits from gameobject and adds spritesheet compatibility.
    /// </summary>
    public class SpriteSheetGameObject : SpriteGameObject
    {
        protected SpriteSheet spriteSheet;

        public SpriteSheetGameObject(string spriteName, int variation=0, int variations=3)
            :base(spriteName)
        {
            if(sprite!=null)
                spriteSheet = new SpriteSheet(Sprite, variation,variations: variations);
        }

        public SpriteSheetGameObject(string spriteName, int variation = 0, int elementWidth = 32, int elementHeigth = 32)
        : base(spriteName)
        {
            if (sprite != null)
                spriteSheet = new SpriteSheet(Sprite, variation, elementWidth, elementHeigth);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(visible)
                spriteSheet.Draw(spriteBatch, gameTime, position);
        }

        public override Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X-(int)spriteSheet.Origin.X, (int)Position.Y-(int)spriteSheet.Origin.Y, spriteSheet.Width, spriteSheet.Height); }
        }

        public bool Mirror
        {
            get { return spriteSheet.Mirror; }
            set { spriteSheet.Mirror = value; }
        }
        
        /// <summary>
        /// Returns the current spritesheet
        /// </summary>
        public SpriteSheet Sheet
        {
            get { return spriteSheet; }
        }

        public void ChangeSpriteSheet(string spriteName, int variation = 0, int variations = 3)
        {
            sprite = AssetManager.GetSprite(spriteName);
            spriteSheet = new SpriteSheet(Sprite, variation, variations: variations);
        }
    }
}
