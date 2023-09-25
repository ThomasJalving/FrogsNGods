using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frogger.GameManagement;
using Frogger.GameManagement.GameObj;
using Frogger.Menu;
using Frogger.GameObjects;
using Microsoft.Xna.Framework;

namespace Frogger.Menu
{
    /// <summary>
    /// Creates a dragging slider that can change a setting
    /// </summary>
    class SliderButton : GameObjectList
    {
        protected Button back, front;
        protected float min, max, minPos, maxPos;
        protected bool dragging;

        public SliderButton(string back, string front, Vector2 position, float minValue, float maxValue, float startValue )
        {
            this.position = position;
            this.back = new Button(back, 0, 1);
            this.back.Position = position;
            this.front = new Button(front, 0, 2);
            minPos = this.back.Position.X +  6 * this.front.BoundingBox.Width;
            maxPos = this.back.Position.X + this.back.BoundingBox.Width - 6 * this.front.BoundingBox.Width - 6;
            min = minValue;
            max = maxValue;
            SetSliderFront(startValue);
            Add(this.back);
            Add(this.front);
        }

        public void SetSliderFront(float startValue)
        {
            float startPos = ((maxPos - minPos) / (max - min)) * (startValue - min);
            this.front.Position = new Vector2(minPos + startPos, this.back.BoundingBox.Height / 2 - this.front.BoundingBox.Height / 2 + 1 + this.back.Position.Y);
        }

        /// <summary>
        /// Handles where the slider is
        /// </summary>
        public void SliderHandling(Cursor cursor, ref float value)
        {
            front.Position = new Vector2(MathHelper.Clamp(cursor.Position.X, minPos, maxPos), front.Position.Y);
            dragging = true;
            ModifiyValue(ref value);
        }

        public bool IsOnSlider
        {
            get { return front.IsOnButton || back.IsOnButton; }
        }

        /// <summary>
        /// Updates the value for the given setting
        /// </summary>
        public void ModifiyValue(ref float value)
        {
            value = (front.Position.X-minPos)/(maxPos-minPos)*(max - min)+min;
        }

        public bool IsDragging
        {
            get { return dragging; }
            set { dragging = value; }
        }
    }
}
