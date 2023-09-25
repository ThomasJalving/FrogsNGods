using Microsoft.Xna.Framework;
using Frogger.GameManagement;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameObjects;
using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.States
{
    /// <summary>
    /// A gamestate that handles all elements that run during gameplay
    /// </summary>
    class GamePlaying : GameObjectList
    {
        protected Level level;
        protected Player player;
        protected Camera cam;
        protected InputHelper inputHelper;

        public GamePlaying(InputHelper inputHelper)
        {
            this.inputHelper = inputHelper;
            level = new Level(inputHelper);
            player = new Player(level);
            Add(player);
            cam = new Camera(496);
            cam.Target = player;         
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            level.Update(gameTime);
            cam.Follow();
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            level.HandleInput(inputHelper);
            base.HandleInput(inputHelper);
            if (inputHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                level.PauseGodTimers();
                Frogger.GameStateManager.SwitchTo("pauseMenu");
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cam.Transform);
            level.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin();
            level.activeGod.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override void Reset()
        {
            level = new Level(inputHelper);
            DeleteChild(player);
            player = new Player(level);
            Add(player);
            cam.Target = player;
        }

        public Level Level
        {
            get { return level; }
        }

        public Player Player
        {
            get { return player; }
        }
    }
}
