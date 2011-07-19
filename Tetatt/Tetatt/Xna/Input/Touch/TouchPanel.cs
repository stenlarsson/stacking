using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Xna.Framework.Input.Touch
{
    public class TouchPanel
    {
        public static GestureType EnabledGestures { get; set; }

        public static TouchCollection GetState()
        {
            return new TouchCollection();
        }

        public static bool IsGestureAvailable { get; set; }

        internal static GestureSample ReadGesture()
        {
            throw new NotImplementedException();
        }
    }
}
