using System;

namespace Microsoft.Xna.Framework.Input
{
    public struct KeyboardState
    {
        internal bool[] keystate;

        public bool IsKeyUp(Keys key)
        {
            return !IsKeyDown(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return keystate[(int)key];
        }
    }
}
