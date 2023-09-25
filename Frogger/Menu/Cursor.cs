using Frogger.GameManagement.GameObj;
using Frogger.GameManagement;

namespace Frogger.Menu
{
    /// <summary>
    /// Creates a cursor that follows the mouse position
    /// </summary>
    class Cursor : SpriteGameObject
    {
        protected string spriteName;

        public Cursor()
        {
            spriteName = "Menu/Cursor";
            sprite = AssetManager.GetSprite(spriteName);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            position.X = inputHelper.MousePosition.X - (Sprite.Width / 2);
            position.Y = inputHelper.MousePosition.Y - (Sprite.Height / 2);
        }
    }
}
