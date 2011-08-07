using System;

namespace Microsoft.Xna.Framework.Input
{
    public struct GamePadState
    {
        public GamePadButtons Buttons
        {
            get { return new GamePadButtons(); }
        }
        public bool IsConnected
        {
            get { return false; }
        }
        public bool IsButtonUp(Buttons button)
        {
            return true;
        }
        public bool IsButtonDown(Buttons button)
        {
            return false;
        }
        public GamePadThumbSticks ThumbSticks
        {
            get { return new GamePadThumbSticks(); }
        }
    }
}
