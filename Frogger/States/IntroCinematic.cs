using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement;
using Frogger.Menu;
using System;

namespace Frogger.States
{
    /// <summary>
    /// A gamestate handling the intro cinematic
    /// </summary>
    class IntroCinematic : GameObjectList
    {
        private Stopwatch timer;
        private SpriteGameObject background, book, ragemeter, slider;
        private AnimatedGameObject tadpoles, frog, blob, exclamationmark, logo;
        private Camera cam;
        private int blobMoves, frogMoves, logoMoves;

        public IntroCinematic()
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

            cam = new Camera();
            timer = new Stopwatch();

            tadpoles = new AnimatedGameObject("Menu/Tadpoles");
            tadpoles.Position = new Vector2(Level.tileWidth * 4, Level.tileHeight * 10) + background.Position;
            tadpoles.LoadAnimation("swimming", "Menu/Tadpoles", 5, 1.0f / 5, false, true);
            Add(tadpoles);

            frog = new AnimatedGameObject("Player/Frog Up");
            frog.Position = new Vector2(Level.tileWidth * 8, Level.tileHeight * 18) + background.Position;
            frog.LoadAnimation("frogupslow", "Player/Frog Up", 20, 0.5f / 20);
            frog.LoadAnimation("frogup", "Player/Frog Up", 20, 0.2f / 20);
            frog.LoadAnimation("frogright", "Player/Frog Side", 20, 0.2f / 20);
            frog.animations["frogright"].Origin = new Vector2(16, 0);
            frog.Velocity = new Vector2(0, -(Level.tileHeight * 2));
            frogMoves = -1;

            blob = new AnimatedGameObject("Menu/Blob Still");
            blob.Position = new Vector2(Level.tileWidth * 4, -Level.tileHeight) + background.Position;
            blob.LoadAnimation("blobstill", "Menu/Blob Still", 2, 0.5f / 2, false, true);
            blob.LoadAnimation("blobup", "Menu/Blob Up", 2, 0.5f / 2, false, true);
            blob.LoadAnimation("blobdown", "Menu/Blob Down", 2, 0.5f / 2, false, true);
            blob.LoadAnimation("blobleft", "Menu/Blob Side", 2, 0.5f / 2, true, true);
            blob.LoadAnimation("blobright", "Menu/Blob Side", 2, 0.5f / 2, false, true);
            blob.Velocity = new Vector2(0, Level.tileHeight * 2);
            blobMoves = -1;

            exclamationmark = new AnimatedGameObject("Menu/Exclamationmark");
            exclamationmark.LoadAnimation("!", "Menu/Exclamationmark", 8, 1.6f / 8);

            logo = new AnimatedGameObject("Menu/Logo1");
            logo.Position = new Vector2(0, 0);
            logo.LoadAnimation("off", "Menu/Logo1", 1, 0.1f / 1);
            logo.LoadAnimation("on", "Menu/Logo2", 1, 0.1f / 1);
            logo.LoadAnimation("end1", "Menu/Logo3-1", 3, 0.6f / 3);
            logo.LoadAnimation("end2", "Menu/Logo3-2", 3, 0.6f / 3);
            logo.LoadAnimation("end3", "Menu/Logo3-3", 3, 0.6f / 3);
            logo.LoadAnimation("end4", "Menu/Logo3-4", 3, 0.6f / 3);
            logoMoves = 0;
            Add(logo);
        }

        /// <summary>
        /// Handles the logo animation
        /// </summary>
        private void Logo()
        {
            switch (logoMoves)
            {
                case 0:
                    logo.StartAnimation("off", 1.8f);
                    timer.Start();
                    logoMoves++;
                    break;
                case 1:
                    if (timer.ElapsedMilliseconds >= 1800)
                    {
                        logo.StartAnimation("on", 0.1f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 2:
                    if (timer.ElapsedMilliseconds >= 100)
                    {
                        logo.StartAnimation("off", 1.8f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 3:
                    if (timer.ElapsedMilliseconds >= 1800)
                    {
                        logo.StartAnimation("on", 0.1f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 4:
                    if (timer.ElapsedMilliseconds >= 100)
                    {
                        logo.StartAnimation("off", 0.1f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 5:
                    if (timer.ElapsedMilliseconds >= 100)
                    {
                        logo.StartAnimation("on", 0.1f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 6:
                    if (timer.ElapsedMilliseconds >= 100)
                    {
                        logo.StartAnimation("off", 0.6f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 7:
                    if (timer.ElapsedMilliseconds >= 600)
                    {
                        logo.StartAnimation("on", 1.0f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 8:
                    if (timer.ElapsedMilliseconds >= 1000)
                    {
                        logo.StartAnimation("end1", 0.6f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 9:
                    if (timer.ElapsedMilliseconds >= 600)
                    {
                        logo.StartAnimation("end2", 0.6f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 10:
                    if (timer.ElapsedMilliseconds >= 600)
                    {
                        logo.StartAnimation("end3", 0.6f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 11:
                    if (timer.ElapsedMilliseconds >= 600)
                    {
                        logo.StartAnimation("end4", 0.6f);
                        timer.Restart();
                        logoMoves++;
                    }
                    break;
                case 12:
                    if (timer.ElapsedMilliseconds >= 600)
                    {
                        DeleteChild(logo);
                        timer.Reset();
                        blobMoves = 0;
                        Add(blob);
                        logoMoves++;
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles the animation of the black blob that steals the tadpoles
        /// </summary>
        private void BlobMovement()
        {
            switch (blobMoves)
            {
                case 0:
                    if (blob.Position.Y >= background.Position.Y + (Level.tileHeight * 4))
                    {
                        blob.Velocity = new Vector2(Level.tileWidth * 4, 0);
                        blobMoves++;
                    }
                    break;
                case 1:
                    if (blob.Position.X >= background.Position.X + (Level.tileWidth * 5))
                    {
                        blob.Velocity = Vector2.Zero;
                        timer.Start();
                        blobMoves++;
                    }
                    break;
                case 2:
                    if (timer.ElapsedMilliseconds >= 2000)
                    {
                        blob.Velocity = new Vector2(-(Level.tileWidth * 1), 0);
                        timer.Reset();
                        blobMoves++;
                    }
                    break;
                case 3:
                    if (blob.Position.X <= background.Position.X + (Level.tileWidth * 4))
                    {
                        blob.Velocity = new Vector2(0, Level.tileHeight * 2);
                        blobMoves++;
                    }
                    break;
                case 4:
                    if (blob.Position.Y >= background.Position.Y + (Level.tileHeight * 9))
                    {
                        blob.Velocity = Vector2.Zero;
                        timer.Start();
                        blobMoves++;
                    }
                    break;
                case 5:
                    if (timer.ElapsedMilliseconds >= 1500)
                    {
                        blob.Velocity = new Vector2(0.1f, 0);
                        timer.Restart();
                        blobMoves++;
                    }
                    break;
                case 6:
                    if (timer.ElapsedMilliseconds >= 1500)
                    {
                        blob.Velocity = new Vector2(-0.1f, 0);
                        timer.Restart();
                        blobMoves++;
                    }
                    break;
                case 7:
                    if (timer.ElapsedMilliseconds >= 1500)
                    {
                        blob.Velocity = Vector2.Zero;
                        timer.Restart();
                        blobMoves++;
                    }
                    break;
                case 8:
                    if (timer.ElapsedMilliseconds >= 1500)
                    {
                        timer.Restart();
                        DeleteChild(tadpoles);
                        Add(frog);
                        frogMoves = 0;
                        blobMoves++;
                    }
                    break;
                case 9:
                    if (timer.ElapsedMilliseconds >= 500)
                    {
                        blob.Velocity = new Vector2(0, -(Level.tileHeight * 6));
                        timer.Reset();
                        blobMoves++;
                    }
                    break;
            }            
        }

        /// <summary>
        /// Handles the animation of the frog
        /// </summary>
        private void FrogMovement()
        {
            switch (frogMoves)
            {
                case 0:
                    frog.StartAnimation("frogupslow", 1.0f);
                    frogMoves++;
                    break;
                case 1:
                    if (frog.Position.Y <= background.Position.Y + (Level.tileHeight * 16))
                    {
                        frog.Velocity = Vector2.Zero;
                        timer.Start();
                        frogMoves++;
                    }
                    break;
                case 2:
                    if (timer.ElapsedMilliseconds >= 200)
                    {
                        Add(exclamationmark);
                        exclamationmark.Position = new Vector2(24, -12) + frog.Position;
                        exclamationmark.StartAnimation("!");
                        timer.Restart();
                        frogMoves++;
                    }
                    break;
                case 3:
                    if (timer.ElapsedMilliseconds >= 1000)
                    {
                        DeleteChild(exclamationmark);
                        timer.Restart();
                        frogMoves++;
                    }
                    break;
                case 4:
                    if (timer.ElapsedMilliseconds >= 200)
                    {
                        frog.Velocity = new Vector2(Level.tileWidth * 5, 0);
                        frog.StartAnimation("frogright");
                        timer.Restart();
                        frogMoves++;
                    }
                    break;
                case 5:
                    if (timer.ElapsedMilliseconds >= 200)
                    {
                        frog.Velocity = new Vector2(0, -(Level.tileHeight * 5));
                        frog.StartAnimation("frogup", 1.0f);
                        timer.Restart();
                        frogMoves++;
                    }
                    break;
                case 6:
                    if (timer.ElapsedMilliseconds >= 1000)
                    {
                        timer.Reset();
                        AssetManager.PlayMusic("Music/Menu");
                        GameEnvironment.GameStateManager.SwitchTo("menuScreen");
                    }
                    break;
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            //Allows you to skip the intro cinematic
            if (inputHelper.KeyDown(Keys.Space))
            {
                AssetManager.PlayMusic("Music/Menu");
                GameEnvironment.GameStateManager.SwitchTo("menuScreen");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Determines what animation to use for the blob depending on his movement direction
            if (blob.Velocity != Vector2.Zero)
            {
                if (blob.Velocity.X > 0 && blob.CurrentAnimation != blob.animations["blobright"])
                    blob.StartAnimation("blobright");
                else if (blob.Velocity.X < 0 && blob.CurrentAnimation != blob.animations["blobleft"])
                    blob.StartAnimation("blobleft");
                else if (blob.Velocity.Y > 0 && blob.CurrentAnimation != blob.animations["blobdown"])
                    blob.StartAnimation("blobdown");
                else if (blob.Velocity.Y < 0 && blob.CurrentAnimation != blob.animations["blobup"])
                    blob.StartAnimation("blobup");
            }
            else if (blob.CurrentAnimation != blob.animations["blobstill"])
            {
                blob.StartAnimation("blobstill");
            }

            //plays the different animation methods
            if (logoMoves < 13)
                Logo();
            if (blobMoves > -1 && blobMoves < 10)
                BlobMovement();
            if (frogMoves > -1 && frogMoves < 7)
                FrogMovement();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: cam.Transform);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
