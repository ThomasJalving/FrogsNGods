using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Linq;
using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frogger.GameManagement
{
    /// <summary>
    /// The gameState for changing your settings
    /// </summary>
    class SettingsManager
    {
        protected Dictionary<string, int> valueSettings;
        protected Dictionary<string, Keys> keyboardSettings;
        protected List<DisplayMode> resolutions;

        /// <summary>
        /// Class that manages lists of settings and settingfiles
        /// </summary>
        public SettingsManager()
        {
            valueSettings = new Dictionary<string, int>();
            keyboardSettings = new Dictionary<string, Keys>();
            resolutions = new List<DisplayMode>();
            KeySettingsReader();
            ValueSettingsReader();
        }

        /// <summary>
        /// Gets a list of all resolutions
        /// </summary>
        public List<DisplayMode> GetResolutions()
        {
            foreach(DisplayMode displayMode in Frogger.Graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                if(displayMode.AspectRatio==16f/9f)
                    resolutions.Add(displayMode);
            }
            return resolutions;
        }

        /// <summary>
        /// Applies settings to the dictionary if neccesary and then writes them to a file
        /// </summary>
        public void ApplySettings()
        {
            SetValue("Volume", (int)(MediaPlayer.Volume * 100));
            if (Frogger.Graphics.IsFullScreen)
                SetValue("FullScreen", 1);
            else
                SetValue("FullScreen", 0);
            KeySettingsWriter();
            ValueSettingsWriter();
        }

        /// <summary>
        /// Retrieves and applies settings from the dictionary and should be called at startup of application
        /// </summary>
        public void ActivateSettings()
        {
            MediaPlayer.Volume = (float)GetValue("Volume") / 100;
            if (GetValue("FullScreen") == 1)
            {
                ToggleFullScreen();
            }
            else
            {
                Frogger.Graphics.PreferredBackBufferHeight = GetValue("WindowedHeigth");
                Frogger.Graphics.PreferredBackBufferWidth = GetValue("WindowedWidth");
                Frogger.Graphics.ApplyChanges();
            }
        }


        //Graphics

        /// <summary>
        /// Sets a resolution for your window if in windowed, otherwise for fullscreen
        /// </summary>
        public void SetResolution(DisplayMode displayMode)
        {
            SetValue("WindowedWidth",displayMode.Width);
            SetValue("WindowedHeigth",displayMode.Height);
            if(!Frogger.Graphics.IsFullScreen)
            {
                Frogger.Graphics.PreferredBackBufferHeight = GetValue("WindowedHeigth");
                Frogger.Graphics.PreferredBackBufferWidth = GetValue("WindowedWidth");
                Frogger.Graphics.ApplyChanges();
            }
        }

        /// <summary>
        /// Toggles from fullscreen to windowedMode using correct windowed resolutions
        /// </summary>
        public void ToggleFullScreen()
        {
            Frogger.Graphics.ToggleFullScreen();
            if (Frogger.Graphics.IsFullScreen)
            {
                Frogger.Graphics.PreferredBackBufferHeight = Frogger.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                Frogger.Graphics.PreferredBackBufferWidth = Frogger.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                Frogger.Graphics.ApplyChanges();
            }
            else
            {
                Frogger.Graphics.PreferredBackBufferHeight = GetValue("WindowedHeigth");
                Frogger.Graphics.PreferredBackBufferWidth = GetValue("WindowedWidth");
                Frogger.Graphics.ApplyChanges();
            }
        }

        /// <summary>
        /// Reads all settings that use the Keys enum from a file
        /// </summary>
        public void KeySettingsReader()
        {
            StreamReader fileReader;
            string[] settings;
            int settingsLine;
            string setting;
            string data;

            settingsLine = File.ReadLines("content/Settings/KeySettings.txt").Count();
            fileReader = new StreamReader("Content/Settings/KeySettings.txt");
            settings = new string[settingsLine];

            for (int i = 0; i < settingsLine; i++)
            {
                settings[i] = fileReader.ReadLine();
            }

            foreach(string settingData in settings)
            {
                setting = settingData.Split('=')[0];
                data = settingData.Split('=')[1];
                Keys key = (Keys)Enum.Parse(typeof(Keys), data);
                keyboardSettings.Add(setting, key);
            }
            fileReader.Close();
        }
        /// <summary>
        /// Writes all settings that use the Keys enum to a file
        /// </summary>
        public void KeySettingsWriter()
        {
            StreamWriter fileWriter;
            string setting;
            string data;

            fileWriter = new StreamWriter("Content/Settings/KeySettings.txt");

            foreach (KeyValuePair<string, Keys> entry in keyboardSettings)
            {
                setting = entry.Key;
                data = entry.Value.ToString();
                if (entry.Key.Equals(keyboardSettings.Keys.Last()))
                    fileWriter.Write(setting + "=" + data);
                else
                    fileWriter.WriteLine(setting + "=" + data);
            }
            fileWriter.Close();
        }

        /// <summary>
        /// Reads all settings that use an int value from a file
        /// </summary>
        public void ValueSettingsReader()
        {
            StreamReader fileReader;
            string[] settings;
            int settingsLine;
            string setting;
            string data;

            settingsLine = File.ReadLines("content/Settings/ValueSettings.txt").Count();
            fileReader = new StreamReader("Content/Settings/ValueSettings.txt");
            settings = new string[settingsLine];

            for (int i = 0; i < settingsLine; i++)
            {
                settings[i] = fileReader.ReadLine();
            }

            foreach (string settingData in settings)
            {
                setting = settingData.Split('=')[0];
                data = settingData.Split('=')[1];

                valueSettings.Add(setting, int.Parse(data));
            }
            fileReader.Close();
        }

        /// <summary>
        /// Writes all settings that use an int value to a file
        /// </summary>
        public void ValueSettingsWriter()
        {
            StreamWriter fileWriter;
            string setting;
            int data;

            fileWriter = new StreamWriter("Content/Settings/ValueSettings.txt");

            foreach (KeyValuePair<string, int> entry in valueSettings)
            {
                setting = entry.Key;
                data = entry.Value;
                fileWriter.WriteLine(setting + "=" + data);
            }
            fileWriter.Close();
        }

        /// <summary>
        /// Sets every setting to the default value and saves settings to file
        /// </summary>
        public void SetDefault()
        {
            SetValue("WindowedHeigth", Frogger.Screen.Y);
            SetValue("WindowedWidth", Frogger.Screen.X);
            SetValue("FullScreen", 0);
            SetValue("Volume", 100);
            SetDefaultKeys();
            ActivateSettings();
            ValueSettingsWriter();
        }

        /// <summary>
        /// Resets just the bound Keys to default and writes to settingsfile
        /// </summary>
        public void SetDefaultKeys()
        {
            SetKey("Up", Keys.W);
            SetKey("UpSecondary", Keys.Up);
            SetKey("Down", Keys.S);
            SetKey("DownSecondary", Keys.Down);
            SetKey("Left", Keys.A);
            SetKey("LeftSecondary", Keys.Left);
            SetKey("Rigth", Keys.D);
            SetKey("RigthSecondary", Keys.Right);
            SetKey("DebugRealmDown", Keys.I);
            SetKey("DebugRealmUp", Keys.U);
            SetKey("DebugTeleport", Keys.O);
            SetKey("DebugInvincible", Keys.J);
            SetKey("DebugShield", Keys.K);
            KeySettingsWriter();
        }

        public void SetValue(string key, int value)
        {
            valueSettings[key] = value;
        }

        public int GetValue(string key)
        {
            return valueSettings[key];
        }

        public ICollection<string> GetKeyListKeys()
        {
            return keyboardSettings.Keys;
        }

        public void SetKey(string key, Keys keyboardKey)
        {
            keyboardSettings[key] = keyboardKey;
        }

        public Keys GetKey(string key)
        {
            return keyboardSettings[key];
        }
    }
}
