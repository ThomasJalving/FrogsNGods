using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Frogger.GameManagement.LevelGenerator;
using Frogger.GameManagement.GameObj;
using Frogger.GameManagement;
using Frogger.Menu;

namespace Frogger.States
{
    /// <summary>
    /// A gamestate for editing game settings
    /// </summary>
    class SettingsMenu : GameObjectList
    {
        private Button backButton, apply, fullScreenToggle, reset, resetkeys;
        private SpriteGameObject book, background, ragemeter, slider;
        private SliderButton volume;
        private bool buttonLock;
        private float buttonLockTime;
        private Camera cam;
        private Cursor mouse;
        private KeyRebinder keyRebinder;
        private ResolutionSetter resolutionSetter;

        public SettingsMenu()
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

            backButton = new Button("Menu/Button Return to Menu", 0, 2);
            backButton.Position = new Vector2(background.Sprite.Width - backButton.BoundingBox.Width + background.Position.X, 540 - backButton.Sprite.Height);
            Add(backButton);

            apply = new Button("Menu/Button Save Changes", 0, 2, "");
            apply.Position = new Vector2(0 + background.Position.X, 540 - apply.Sprite.Height);
            Add(apply);

            reset = new Button("Menu/Button Reset", 0, 2, "");
            reset.Position = new Vector2(0 + background.Position.X, 540 - reset.Sprite.Height * 3);
            Add(reset);

            resetkeys = new Button("Menu/Button Reset Keys", 0, 2, "");
            resetkeys.Position = new Vector2(0 + background.Position.X, 540 - resetkeys.Sprite.Height * 2);
            Add(resetkeys);

            volume = new SliderButton("Menu/Volume Bar", "Menu/Volume Slider", new Vector2(background.Position.X + 24, 0), 0f, 1f, MediaPlayer.Volume);
            Add(volume);

            fullScreenToggle = new Button("Menu/Button Fullscreen", 0, 2, "");
            fullScreenToggle.Position = new Vector2(background.Position.X + background.BoundingBox.Width - fullScreenToggle.BoundingBox.Width - 24, 0);
            Add(fullScreenToggle);

            keyRebinder = new KeyRebinder(Frogger.SettingsManager.GetKeyListKeys(), volume.Position + new Vector2(-24, 64));
            Add(keyRebinder);

            resolutionSetter = new ResolutionSetter(Frogger.SettingsManager.GetResolutions(), fullScreenToggle.Position + new Vector2(-24, 64));
            Add(resolutionSetter);

            buttonLock = false;
            buttonLockTime = 50f;

            cam = new Camera();

            mouse = new Cursor();
            Add(mouse);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (!keyRebinder.InputLock)
            {
                if (inputHelper.MouseLeftButtonPressed() && backButton.IsOnButton)
                {
                    Frogger.GameStateManager.SwitchTo("last");
                }


                if (inputHelper.MouseLeftButtonPressed() && apply.IsOnButton)
                {
                    Frogger.SettingsManager.ApplySettings();
                }

                if (inputHelper.MouseLeftButtonPressed() && reset.IsOnButton)
                {
                    Frogger.SettingsManager.SetDefault();
                    keyRebinder.Next();
                    keyRebinder.Previous();
                    volume.SetSliderFront(MediaPlayer.Volume);
                }

                if (inputHelper.MouseLeftButtonPressed() && resetkeys.IsOnButton)
                {
                    Frogger.SettingsManager.SetDefaultKeys();
                    keyRebinder.Next();
                    keyRebinder.Previous();
                }

                if (inputHelper.MouseLeftButtonPressed() && fullScreenToggle.IsOnButton)
                {
                    Frogger.SettingsManager.ToggleFullScreen();
                }

                if (volume.IsDragging && !inputHelper.MouseLeftButtonDown())
                    volume.IsDragging = false;
                if ((inputHelper.MouseLeftButtonPressed() && volume.IsOnSlider) || (inputHelper.MouseLeftButtonDown() && volume.IsDragging))
                {
                    float volumeValue = MediaPlayer.Volume;
                    volume.SliderHandling(mouse, ref volumeValue);
                    MediaPlayer.Volume = volumeValue;
                }

                if ((keyRebinder.IsOnNext || keyRebinder.IsOnPrevious || keyRebinder.IsOnSet) && inputHelper.MouseLeftButtonPressed())
                {
                    if (keyRebinder.IsOnNext)
                    {
                        keyRebinder.Next();
                    }
                    else if (keyRebinder.IsOnPrevious)
                    {
                        keyRebinder.Previous();
                    }
                    else
                    {
                        keyRebinder.Set();
                    }
                }

                if ((resolutionSetter.IsOnNext || resolutionSetter.IsOnPrevious || resolutionSetter.IsOnSet) && inputHelper.MouseLeftButtonPressed())
                {
                    if (resolutionSetter.IsOnNext)
                    {
                        resolutionSetter.Next();
                    }
                    else if (resolutionSetter.IsOnPrevious)
                    {
                        resolutionSetter.Previous();
                    }
                    else
                    {
                        resolutionSetter.Set();
                    }
                }

                if (!inputHelper.MouseLeftButtonDown())
                    buttonLock = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (buttonLock)
            {
                if(buttonLockTime<=0)
                {
                    buttonLock = false;
                    buttonLockTime = 50f;
                }
                else
                buttonLockTime -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
                buttonLockTime = 50f;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: cam.Transform);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
