using System;

namespace Microsoft.Xna.Framework.Input
{
    public class Keyboard
    {
        static internal bool[] keys =
            new bool[Enum.GetValues(typeof(OpenTK.Input.Key)).Length];

        public static KeyboardState GetState()
        {
            return new KeyboardState { keystate = (bool[])keys.Clone() };
        }

        public static KeyboardState GetState(PlayerIndex playerIndex)
        {
            return GetState();
        }
    }
}
