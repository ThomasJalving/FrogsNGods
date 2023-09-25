using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Frogger.GameManagement
{
    /// <summary>
    /// Deals with KeyBoard, Mouse and Speech input
    /// </summary>
    public partial class InputHelper
    {
        //keeps track of the current and previous states of both mouse and keyboard
        KeyboardState currentKeyboard, previousKeyboard;
        MouseState currentMouse, previousMouse;

        public InputHelper()
        {
            speechPrompts = new List<string>();
            Speech = new SpeechHelper(this);
        }

        public void Update()
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
        }

        //returns true if the key was down last update but has been released in the current update
        public bool KeyPressed(Keys key)
        {
            return currentKeyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        //returns true if the key is currently held down
        public bool KeyDown(Keys key)
        {
            return currentKeyboard.IsKeyDown(key);
        }

        public bool MouseLeftButtonPressed()
        {
            return currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;
        }

        public bool MouseLeftButtonDown()
        {
            return currentMouse.LeftButton == ButtonState.Pressed;
        }

        //returns the mouse position
        public Vector2 MousePosition
        {
            get { return new Vector2((float)currentMouse.X / Frogger.Graphics.PreferredBackBufferWidth * Frogger.Screen.X, (float)currentMouse.Y / Frogger.Graphics.PreferredBackBufferHeight * Frogger.Screen.Y); }
        }

        public Keys[] GetInputKey()
        {
            return currentKeyboard.GetPressedKeys();
        }

        //For typing initials
        public bool TryConvertKeyboardInput(out char key, bool neverCapital)
        {
            Keys[] keys = currentKeyboard.GetPressedKeys();
            bool shift = (currentKeyboard.IsKeyDown(Keys.LeftShift) || currentKeyboard.IsKeyDown(Keys.RightShift)) && !neverCapital;

            if (keys.Length > 0 && !previousKeyboard.IsKeyDown(keys[0]))
            {
                switch (keys[0])
                {
                    //Alphabet keys
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;

                    //Decimal keys
                    case Keys.D0:  key = '0';  return true;
                    case Keys.D1:  key = '1';  return true;
                    case Keys.D2:  key = '2';  return true;
                    case Keys.D3:  key = '3';  return true;
                    case Keys.D4:  key = '4';  return true;
                    case Keys.D5:  key = '5';  return true;
                    case Keys.D6:  key = '6';  return true;
                    case Keys.D7:  key = '7';  return true;
                    case Keys.D8:  key = '8';  return true;
                    case Keys.D9:  key = '9';  return true;
                }
            }

            key = (char)0;
            return false;
        }

    }
}
