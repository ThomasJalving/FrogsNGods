using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frogger.Menu;
using Frogger.GameManagement.GameObj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frogger.GameManagement;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.Menu
{
    /// <summary>
    /// Allows rebinding all keys from a list of needed keys
    /// </summary>
    class KeyRebinder : SettingSetter
    {
        protected bool inputLock;
        protected Keys current;
        protected ICollection<string> keys;

        public KeyRebinder(ICollection<string> keys, Vector2 position)
            :base(position, "Menu/Button Set Buttons")
        {
            current = Frogger.SettingsManager.GetKey(keys.ElementAt(keyNumber));
            this.keys = keys;
            currentDisplay.Text = keys.ElementAt(keyNumber) + ": " + current;
        }

        public override void Set()
        {
            inputLock = true;
            currentDisplay.Text = "Press a key for " + keys.ElementAt(keyNumber);
        }

        public override void Next()
        {
            keyNumber += 1;
            if (keyNumber >= keys.Count)
                keyNumber = 0;
            current = Frogger.SettingsManager.GetKey(keys.ElementAt(keyNumber));
            currentDisplay.Text = keys.ElementAt(keyNumber) + ": " + current;
        }

        public override void Previous()
        {
            keyNumber -= 1;
            if (keyNumber < 0)
                keyNumber = keys.Count-1;
            current = Frogger.SettingsManager.GetKey(keys.ElementAt(keyNumber));
            currentDisplay.Text = keys.ElementAt(keyNumber) + ": " + current;
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (inputLock && inputHelper.GetInputKey().Count()>0)
            {
                Frogger.SettingsManager.SetKey(keys.ElementAt(keyNumber), inputHelper.GetInputKey()[0]);
                current = Frogger.SettingsManager.GetKey(keys.ElementAt(keyNumber));
                currentDisplay.Text = keys.ElementAt(keyNumber) + ": " + current;
                inputLock = false;
            }
        }

        public bool InputLock
        {
            get { return inputLock; }
        }
    }
}
