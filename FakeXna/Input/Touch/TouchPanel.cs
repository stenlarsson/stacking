using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Xna.Framework.Input.Touch
{
    public class TouchPanel
    {
        static TouchPanel()
        {
            IsGestureAvailable = false;
            EnabledGestures = GestureType.None;
        }

        public static GestureType EnabledGestures { get; set; }

        public static TouchCollection GetState()
        {
            return new TouchCollection();
        }

        public static bool IsGestureAvailable { get; private set; }

        public static GestureSample ReadGesture()
        {
            throw new NotImplementedException();
        }
    }
}
