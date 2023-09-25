using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement;
using Frogger.Menu;
using System;

namespace Frogger.States
{
    /// <summary>
    /// The main menu gamestate with buttons to start playing or go to other menus
    /// </summary>
    class MenuScreen : GameObjectList
    {
        private Button playButton, Highscore, ExitButton, Settings, homingButton, ChunkEditor;
        private SpriteGameObject background, book, ragemeter, slider, title;
        private AnimatedGameObject frog;
        private Camera cam;
        private int previousButton, jumpLength;
        private bool arrived;

        public MenuScreen()
        {
            book = new SpriteGameObject("Menu/Book");
            book.Position = new Vector2(1, 1);
            Add(book);

            background = new SpriteGameObject("Menu/Background");
            background.Position = new Vector2(book.Position.X + book.Sprite.Width, book.Position.Y - 16);
            Add(background);

            ragemeter = new SpriteGameObject("Menu/Ragemeter");
            ragemeter.Position = new Vector2(background.Position.X + background.Sprite.Width, background.Position.Y + 16);
            Add(ragemeter);

            slider = new SpriteGameObject("Menu/Slider");
            slider.Position = new Vector2(ragemeter.Position.X + 3, ragemeter.Sprite.Height / 2);
            Add(slider);

            title = new SpriteGameObject("Menu/Title");
            title.Position = new Vector2(book.Position.X, book.Position.Y - 36);
            Add(title);

            playButton = new Button("Menu/Button Start", 0, 2);
            playButton.Position = new Vector2(Level.tileWidth * 4 + book.Sprite.Width, Level.tileHeight * 11 + background.Position.Y - 1);
            Add(playButton);

            Highscore = new Button("Menu/Button Highscores", 0, 2);
            Highscore.Position = new Vector2(0, Level.tileHeight) + playButton.Position;
            Add(Highscore);

            ExitButton = new Button("Menu/Button Exit", 0, 2);
            ExitButton.Position = new Vector2(0, Level.tileHeight) + Highscore.Position;
            Add(ExitButton);

            Settings = new Button("Menu/Button Settings", 0, 2);
            Settings.Position = new Vector2(ExitButton.BoundingBox.Width - Level.tileWidth, ExitButton.BoundingBox.Height + 2) + ExitButton.Position;
            Add(Settings);

            ChunkEditor = new Button("Menu/Button Chunkeditor", 0, 2);
            ChunkEditor.Position = new Vector2(-Level.tileWidth, 0) + Settings.Position;
            Add(ChunkEditor);

            homingButton = playButton;

            jumpLength = 1;
            frog = new AnimatedGameObject("Player/Frog Down");
            frog.Position = new Vector2(playButton.Position.X + playButton.Sheet.Width, playButton.Position.Y);
            frog.LoadAnimation("up", "Player/Frog Up", 20, 0.2f / 20);
            frog.LoadAnimation("down", "Player/Frog Down", 20, 0.2f / 20);
            frog.StartAnimation("down");
            previousButton = 1;
            Add(frog);

            cam = new Camera();
            Add(new Cursor());
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            //Switches to the other gamestates depending on where the player clicks
            if (inputHelper.MouseLeftButtonPressed() && playButton.IsOnButton)
            {
                AssetManager.PlayMusic("Music/Forest");
                GameEnvironment.GameStateManager.SwitchTo("gamePlaying");
                if (GameEnvironment.GameStateManager.GetGameState("gamePlaying") is GamePlaying gamePlaying)
                    gamePlaying.Level.activeGod.questionDelay.Start();
            }

            if (inputHelper.MouseLeftButtonPressed() && Highscore.IsOnButton)
            {
                GameEnvironment.GameStateManager.SwitchTo("highScoreScreen");
            }

            if (inputHelper.MouseLeftButtonPressed() && ExitButton.IsOnButton)
            {
                GameEnvironment.ExitGame = true;
            }

            if (inputHelper.MouseLeftButtonPressed() && Settings.IsOnButton)
            {
                GameEnvironment.GameStateManager.SwitchTo("settingsMenu");
            }

            if (inputHelper.MouseLeftButtonPressed() && ChunkEditor.IsOnButton)
            {
                GameEnvironment.GameStateManager.SwitchTo("chunkEditing");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Handles the frog that jumps between buttons on the menuscreen
            if (playButton.IsOnButton && previousButton != 1)
            {                
                if (previousButton == 3)
                {
                    frog.Velocity = new Vector2(0, -(Level.tileHeight / jumpLength) * 5);
                    frog.StartAnimation("up", 0.4f);
                }
                else
                {
                    frog.Velocity = new Vector2(0, -(Level.tileHeight / jumpLength) * 5);
                    frog.StartAnimation("up", 0.2f);
                }
                homingButton = playButton;
                previousButton = 1;
            }

            if (Highscore.IsOnButton && previousButton != 2)
            {                
                if (previousButton == 1)
                {
                    frog.Velocity = new Vector2(0, Level.tileHeight / jumpLength * 5);
                    frog.StartAnimation("down", 0.2f);
                }
                else
                {
                    frog.Velocity = new Vector2(0, -(Level.tileHeight / jumpLength) * 5);
                    frog.StartAnimation("up", 0.2f);
                }
                homingButton = Highscore;
                previousButton = 2;
            }

            if (ExitButton.IsOnButton && previousButton != 3)
            {                
                if (previousButton == 1)
                {
                    frog.Velocity = new Vector2(0, Level.tileHeight / jumpLength * 5);
                    frog.StartAnimation("down", 0.4f);
                }
                else
                {
                    frog.Velocity = new Vector2(0, Level.tileHeight / jumpLength * 5);
                    frog.StartAnimation("down", 0.2f);
                }
                homingButton = ExitButton;
                previousButton = 3;
            }

            if(frog.Velocity.Y<0)
            {
                arrived = (frog.Position.Y <= homingButton.Position.Y);
            }
            else
            {
                arrived = (frog.Position.Y >= homingButton.Position.Y);
            }
            if (arrived)
            {
                frog.Velocity = Vector2.Zero;
                frog.Position = new Vector2(homingButton.Position.X + homingButton.Sheet.Width, homingButton.Position.Y);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: cam.Transform);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
