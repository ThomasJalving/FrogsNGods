using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// A class that creates a GameObject that allows you to show text
    /// </summary>
    class TextGameObject : GameObject
    {
        protected SpriteFont font;
        protected Vector2 textSize, drawPosition;
        protected string text;
        protected bool centered;
        protected Color color;

        public TextGameObject(string text, Vector2 position, string font ="UI/QuestionFont", bool centered=false, Color color = default(Color))
        {
            this.font = AssetManager.GetSpriteFont(font);
            this.position = position;
            this.centered = centered;
            this.color = color;
            this.position = position;
            Text = text;
        }

        /// <summary>
        /// Allows you to get the current text this element displays or to change it
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                CalculateDrawPosition();
            }
        }

        /// <summary>
        /// Calculates where the text needs to be drawn depending on alignment
        /// </summary>
        protected void CalculateDrawPosition()
        {
            textSize = font.MeasureString(text);
            if (centered)
            {
                drawPosition = position - new Vector2(textSize.X / 2, 0);
            }
            else
                drawPosition = position;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, drawPosition, color);
        }

        public Vector2 Size
        {
            get { return font.MeasureString(text); }
        }

        /// <summary>
        /// Makes it so a string doesn't pass a given X value, but instead continues on the next line
        /// </summary>
        public void BoxText(string textToWrite, float limitX, bool addToCurrentText = false)
        {
            TextGameObject dummy = new TextGameObject("", Vector2.Zero);
            if (!addToCurrentText)
                text = "";

            foreach (char c in textToWrite)
            {
                if (Position.X + dummy.Size.X > limitX)                                             //checks if the line has become too long
                {
                    string lastWord = "";
                    char spaceCheck = dummy.Text[dummy.Text.Length - 1];
                    while (spaceCheck != ' ')                                                       //searches for a space, so no words will be cut off at the end of the line
                    {
                        lastWord += spaceCheck;                                                     //stores the characters that are deleted until a space is found
                        dummy.Text = dummy.Text.Remove(dummy.Text.Length - 1);
                        text = text.Remove(text.Length - 1);
                        spaceCheck = dummy.Text[dummy.Text.Length - 1];
                    }
                    lastWord = Reverse(lastWord);

                    text = $@"{text}
{lastWord}{c}";                                                                                     //puts the last word back in the string one line lower and adds the next character from 'store'
                    dummy.Text = lastWord;                                                          //resets the Size.X of the dummy to measure the length of the next line
                }
                else
                {
                    text += c;
                    dummy.Text += c;
                }
            }
        }

        /// <summary>
        /// Returns the reverse of a string
        /// </summary>
        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
