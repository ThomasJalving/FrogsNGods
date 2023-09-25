using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Frogger.GameManagement.LevelGenerator;

namespace Frogger.GameManagement
{
    class GameEnvironment : Game
    {
        protected SpriteBatch spriteBatch;
        protected InputHelper inputHelper;

        protected static GraphicsDeviceManager graphics;
        protected static GameStateManager gameStateManager;
        protected static SettingsManager settingsManager;
        protected static Point screen;
        protected static bool exitGame;
        protected static Random random;

        public GameEnvironment()
        {
            graphics = new GraphicsDeviceManager(this);
            inputHelper = new InputHelper();
            gameStateManager = new GameStateManager();
            settingsManager = new SettingsManager();
            random = new Random();
            screen = new Point(960, 540);
        }

        public static GameStateManager GameStateManager
        {
            get { return gameStateManager; }
        }

        public static SettingsManager SettingsManager
        {
            get { return settingsManager; }
        }

        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public static Point Screen
        {
            get { return screen; }
            set { screen = value; }
        }

        public static bool ExitGame
        {
            set { exitGame = value; }
        }

        public static Random Random
        {
            get { return random; }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetManager.contentManager = Content;
            settingsManager.ActivateSettings();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected void HandleInput()
        {
            inputHelper.Update();
            if (exitGame)
                Exit();
            gameStateManager.HandleInput(inputHelper);
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();
            gameStateManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            gameStateManager.Draw(gameTime, spriteBatch);
            base.Draw(gameTime);
        }
    }
}
