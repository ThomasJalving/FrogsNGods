using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogger.Menu
{
    /// <summary>
    /// Allows setting the resolution from a list of allowable displaymodes
    /// </summary>
    class ResolutionSetter : SettingSetter
    {
        protected List<DisplayMode> resolutions;
        protected string showCurrent;

        public ResolutionSetter(List<DisplayMode> resolutions, Vector2 position)
            :base(position, "Menu/Button Apply Changes")
        {
            this.resolutions = resolutions;
            if (resolutions[keyNumber].Width == Frogger.SettingsManager.GetValue("WindowedWidth") && resolutions[keyNumber].Height == Frogger.SettingsManager.GetValue("WindowedHeigth"))
                showCurrent = "Current:";
            currentDisplay.Text = showCurrent + resolutions[keyNumber].Width + "X" + resolutions[keyNumber].Height;
        }

        public override void Set()
        {
            showCurrent = "Current:";
            Frogger.SettingsManager.SetResolution(resolutions[keyNumber]);
            currentDisplay.Text = showCurrent + resolutions[keyNumber].Width + "X" + resolutions[keyNumber].Height;
        }

        public override void Next()
        {
            keyNumber += 1;
            if (keyNumber >= resolutions.Count)
                keyNumber = 0;
            if (resolutions[keyNumber].Width == Frogger.SettingsManager.GetValue("WindowedWidth") && resolutions[keyNumber].Height == Frogger.SettingsManager.GetValue("WindowedHeigth"))
                showCurrent = "Current:";
            else
                showCurrent = "";
            currentDisplay.Text = showCurrent + resolutions[keyNumber].Width + "X" + resolutions[keyNumber].Height;
        }

        public override void Previous()
        {
            keyNumber -= 1;
            if (keyNumber < 0)
                keyNumber = resolutions.Count-1;
            if (resolutions[keyNumber].Width == Frogger.SettingsManager.GetValue("WindowedWidth") && resolutions[keyNumber].Height == Frogger.SettingsManager.GetValue("WindowedHeigth"))
                showCurrent = "Current:";
            else
                showCurrent = "";
            currentDisplay.Text = showCurrent + resolutions[keyNumber].Width + "X" + resolutions[keyNumber].Height;
        }
    }
}
