using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frogger.Menu;
using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frogger.GameManagement;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.Menu
{
    /// <summary>
    /// A base class for setting a setting from a list
    /// </summary>
    class SettingSetter : GameObjectList
    {
        protected Button previous, set, next;
        protected int keyNumber;
        protected TextGameObject currentDisplay;
        protected SpriteGameObject currentBack;

        public SettingSetter(Vector2 position, string button)
        {
            this.position = position;
            previous = new Button("Menu/Button Back");
            previous.Position = position;
            set = new Button(button);
            set.Position = previous.Position + new Vector2(previous.BoundingBox.Width-8, -set.BoundingBox.Height / 2);
            next = new Button("Menu/Button Back", mirrored: true);
            next.Position = previous.Position + new Vector2(previous.BoundingBox.Width + set.BoundingBox.Width-16, 0);
            keyNumber = 0;
            currentDisplay = new TextGameObject("", set.Position + new Vector2(set.BoundingBox.Width / 2, set.BoundingBox.Height), centered: true, color: Color.Black);
            currentBack = new SpriteGameObject("Menu/Textbox 5x1");
            currentBack.Position = set.Position + new Vector2(0, set.BoundingBox.Height - 4);
            Add(previous);
            Add(set);
            Add(next);
            Add(currentBack);
            Add(currentDisplay);
        }

        public virtual void Set()
        { }

        public virtual void Next()
        { }

        public virtual void Previous()
        { }

        public bool IsOnNext
        {
            get { return next.IsOnButton; }
        }

        public bool IsOnPrevious
        {
            get { return previous.IsOnButton; }
        }

        public bool IsOnSet
        {
            get { return set.IsOnButton; }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

        }

    }
}
