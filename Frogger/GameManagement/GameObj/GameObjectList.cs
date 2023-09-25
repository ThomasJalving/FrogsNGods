using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.GameManagement.GameObj
{
    public class GameObjectList : GameObject
    {
        /// <summary>
        /// Manages a list of GameObjects
        /// </summary>
        protected List<GameObject> children;

        public GameObjectList()
        {
            children = new List<GameObject>();
        }

        public List<GameObject> Children
        {
            get { return children; }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                children[i].HandleInput(inputHelper);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach(GameObject obj in children)
            {
                obj.Update(gameTime);
            }
            CheckChildren();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (GameObject obj in children)
            {
                obj.Draw(gameTime, spriteBatch);
            }
        }

        public void Add(GameObject obj)
        {
            obj.Parent = this;
            children.Add(obj);
        }

        public void CheckChildren()
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null && children[i].Deletable)
                {
                    DeleteChild(children[i]);
                    i -= 1;
                }
            }
        }

        public void DeleteChild(GameObject obj)
        {
            children.Remove(obj);
        }
    }
}
