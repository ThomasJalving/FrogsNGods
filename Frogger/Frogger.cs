using Frogger.GameManagement;
using Frogger.GameManagement.LevelGenerator;
using Microsoft.Xna.Framework;
using Frogger.States;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger
{
    class Frogger : GameEnvironment
    {
        //start of the application
        [STAThread]
        static void Main()
        {
            Frogger game = new Frogger();
            game.Run();
        }

        protected RenderTarget2D renderTarget;

        public Frogger()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            graphics.PreferredBackBufferHeight = screen.Y;
            graphics.PreferredBackBufferWidth = screen.X; //add room on right side for the god dialogue
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            renderTarget = new RenderTarget2D(GraphicsDevice , screen.X, screen.Y);
            gameStateManager.AddGameState("introCinematic", new IntroCinematic());
            gameStateManager.AddGameState("menuScreen", new MenuScreen());
            gameStateManager.AddGameState("gamePlaying", new GamePlaying(inputHelper));
            gameStateManager.AddGameState("gameOver", new GameOver());
            gameStateManager.AddGameState("highScoreScreen", new HighScoreScreen());
            gameStateManager.AddGameState("settingsMenu", new SettingsMenu());
            gameStateManager.AddGameState("chunkEditing", new ChunkEditing());
            gameStateManager.AddGameState("shop", new Shop());
            gameStateManager.AddGameState("pauseMenu", new PauseMenu());
            gameStateManager.SwitchTo("introCinematic");
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);          
        }

        /// <summary>
        /// Renders every game element to a rendertarget
        /// </summary>
        protected void DrawSceneToTexture(GameTime gameTime)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Renders and scales the rendertarget to the screen
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            DrawSceneToTexture(gameTime);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.PointClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);
            if(graphics.IsFullScreen)
            {
                int adjustedSide;
                if(graphics.PreferredBackBufferWidth/graphics.PreferredBackBufferHeight < 16f/9f)
                {
                    adjustedSide = (int)(graphics.PreferredBackBufferWidth / 16f * 9f);
                    spriteBatch.Draw(renderTarget, new Rectangle(0, (graphics.PreferredBackBufferHeight - adjustedSide) / 2, graphics.PreferredBackBufferWidth, adjustedSide), Color.White);
                }
                else if(graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight > 16f / 9f)
                {
                    adjustedSide = (int)(graphics.PreferredBackBufferHeight / 9f * 16F);
                    spriteBatch.Draw(renderTarget, new Rectangle((graphics.PreferredBackBufferWidth - adjustedSide) / 2, 0, adjustedSide, graphics.PreferredBackBufferHeight), Color.White);
                }
                else
                    spriteBatch.Draw(renderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            }
            else
                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();
        }
    }
}