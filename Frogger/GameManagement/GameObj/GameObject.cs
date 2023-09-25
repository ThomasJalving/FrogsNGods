using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.GameManagement.GameObj
{
    /// <summary>
    /// The basic GameObject
    /// </summary>
    public abstract class GameObject : IGameLoopObject
    {
        protected GameObjectList parent;
        protected Vector2 position, velocity;
        protected bool visible, deletable;

        public GameObject()
        {
            position = Vector2.Zero;
            velocity = Vector2.Zero;
            visible = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public virtual Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public virtual Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 0, 0);
            }
        }

        public virtual GameObjectList Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public bool Deletable
        {
            get { return deletable; }
            set { deletable = value; }
        }

        public virtual void HandleInput(InputHelper inputHelper)
        {
        }

        public virtual void Reset()
        {
        }
    }
}
