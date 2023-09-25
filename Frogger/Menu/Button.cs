using Frogger.GameManagement;
using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.Menu
{
    /// <summary>
    /// Creates a button
    /// </summary>
    class Button : SpriteSheetGameObject
    {
        protected int variation, elementWidth, elementHeight;
        protected Vector2 stringLength;
        protected string buttonText;
        protected bool onButton;
        protected SpriteFont buttonFont;

        public Button(string spriteName, int variation = 0, int variations = 2, string text = null, bool mirrored = false, string fontPath = "UI/ButtonFont")
            : base(spriteName, variation, variations)
        {
            this.variation = variation;
            buttonFont = AssetManager.GetSpriteFont(fontPath);
            elementWidth = Sprite.Width/variations;                
            elementHeight = Sprite.Height;

            if (sprite != null)
                spriteSheet = new SpriteSheet(Sprite, variation, variations: variations, mirrored: mirrored);

            if (text != null)
            {
                buttonText = text;
                stringLength = buttonFont.MeasureString(buttonText);
            }
        }

        public bool IsOnButton
        {
            get { return onButton; }
        }

        /// <summary>
        /// Shows if a button is being hovered over
        /// </summary>
        public override void HandleInput(InputHelper inputHelper)
        {
            if (BoundingBox.Contains((int)inputHelper.MousePosition.X, (int)inputHelper.MousePosition.Y))
            {

                if (spriteSheet.SheetColumns == 2)
                {
                    variation = 1;
                    spriteSheet.Variation = variation;
                }
                onButton = true;
            }
            else
            {
                variation = 0;
                if (spriteSheet.SheetColumns == 2)
                    spriteSheet.Variation = variation;
                onButton = false;
            }
        }

        public override Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, elementWidth, elementHeight); }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if(buttonText!=null)
                spriteBatch.DrawString(buttonFont, buttonText, position + new Vector2(elementWidth / 2, elementHeight / 2 + 3) - stringLength / 2, Color.Black);
        }

        /// <summary>
        /// Changes which text the button shows (only applies to text written with the font)
        /// </summary>
        public void ChangeText(string NewText)
        {
            buttonText = NewText;
            stringLength = buttonFont.MeasureString(buttonText);
        }

        public string GetText
        {
            get { return buttonText; }
        }

    }
}
