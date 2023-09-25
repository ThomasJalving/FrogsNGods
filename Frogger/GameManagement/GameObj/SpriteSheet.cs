using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// A class that holds a texture2d spritesheet and can draw a specific part of this texture.
    /// </summary>
    public class SpriteSheet
    {
        protected Texture2D sheet;
        protected Rectangle source;
        protected int variation, step, sheetColumns, sheetRows;
        protected bool mirror;
        protected Vector2 origin;

        public SpriteSheet(Texture2D sprite, int spriteVariation=0, int variations=1, bool mirrored = false )
        {
            sheet = sprite;
            sheetColumns = variations;
            sheetRows = 1;
            step = 0;
            variation = spriteVariation;
            mirror = mirrored;
            Recalculate();
        }

        public SpriteSheet(Texture2D sprite, int spriteVariation = 0, int elementWidth = 32, int elementHeigth = 32, bool mirrored = false, int column = 0)
        {
            sheet = sprite;
            sheetColumns = sheet.Width / elementWidth;
            sheetRows = sheet.Height / elementHeigth;
            step = column;
            variation = spriteVariation;
            mirror = mirrored;
            Recalculate();
        }

        /// <summary>
        /// Draws the current part of the spritesheet at the requested position.
        /// The sprite can be mirrored.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            if (mirror)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Recalculate();
            spriteBatch.Draw(sheet, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }

        /// <summary>
        /// Recalculates the source rectangles values.
        /// </summary>
        public void Recalculate()
        {
            source = new Rectangle(variation * Width, step * Height, Width, Height);
        }

        /// <summary>
        /// Gets the width of an element in the sheet
        /// </summary>
        public int Width
        {
            get
            { return sheet.Width / sheetColumns; }
        }

        /// <summary>
        /// Gets the height of an element in the sheet
        /// </summary>
        public int Height
        {
            get
            { return sheet.Height / sheetRows; }
        }

        /// <summary>
        /// Returns or sets the mirror bool.
        /// </summary>
        public bool Mirror
        {
            get { return mirror; }
            set { mirror = value; }
        }

        /// <summary>
        /// Allows an origin to be set for sheets with different dimension for a sheet element.
        /// </summary>
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        /// <summary>
        /// Returns the current sheet texture2d.
        /// </summary>
        public Texture2D Sheet
        {
            get { return sheet; }
        }

        /// <summary>
        /// returns source rectangle
        /// </summary>
        public Rectangle Source
        {
            get { return source; }
        }

        /// <summary>
        /// Allows a different sheet element to be chosen or to get the current element.
        /// If the element is changed the source rectangle will also change.
        /// </summary>
        public int Variation
        {
            get { return variation; }
            set
            {
                variation = value;
                Recalculate();
            }
        }

        /// <summary>
        /// Returns the amount of sheetColumns in the sheet
        /// </summary>
        public int SheetColumns
        {
            get { return sheetColumns; }
        }
    }
}
