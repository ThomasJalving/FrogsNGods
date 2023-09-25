using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement;
using Frogger.Menu;

namespace Frogger.States
{
    /// <summary>
    /// A gamestate to go to while pausing the game
    /// </summary>
    class PauseMenu : GameObjectList
    {
        protected Button button1, button2, button3, button4;
        protected Camera cam;

        public PauseMenu()
        {
            button1 = new Button("Menu/Button Resume", 0, 2);
            button1.Position = new Vector2(Level.tileWidth * 5 + 496, Level.tileHeight * 7);
            Add(button1);

            button2 = new Button("Menu/Button Restart", 0, 2);
            button2.Position = new Vector2(Level.tileWidth * 5 + 496, Level.tileHeight * 8);
            Add(button2);

            button3 = new Button("Menu/Button Return to Menu", 0, 2);
            button3.Position = new Vector2(Level.tileWidth * 5 + 496, Level.tileHeight * 9);
            Add(button3);

            button4 = new Button("Menu/Button Settings", 0, 2);
            button4.Position = new Vector2(-button4.BoundingBox.Width + Level.ChunkWidth * Level.tileWidth + 496, 0);
            Add(button4);

            cam = new Camera();
            Add(new Cursor());
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (inputHelper.MouseLeftButtonPressed() && button2.IsOnButton)
            {
                AssetManager.PlayMusic("Music/Forest");
                GameEnvironment.GameStateManager.GetGameState("gamePlaying").Reset();
                GameEnvironment.GameStateManager.SwitchTo("gamePlaying");
            }

            if (inputHelper.MouseLeftButtonPressed() && button3.IsOnButton)
            {
                AssetManager.PlayMusic("Music/Menu");
                GameEnvironment.GameStateManager.GetGameState("gamePlaying").Reset();
                GameEnvironment.GameStateManager.SwitchTo("menuScreen");
            }

            if (inputHelper.MouseLeftButtonPressed() && button4.IsOnButton)
            {
                GameEnvironment.GameStateManager.SwitchTo("settingsMenu");
            }

            if (inputHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape) || (inputHelper.MouseLeftButtonPressed() && button1.IsOnButton))
            {
                if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying gamePlaying)
                    gamePlaying.Level.PauseGodTimers(true);
                Frogger.GameStateManager.SwitchTo("gamePlaying");
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameEnvironment.GameStateManager.GetGameState("gamePlaying").Draw(gameTime, spriteBatch);

            spriteBatch.Begin(transformMatrix: cam.Transform);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
