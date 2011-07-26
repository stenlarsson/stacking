using System;

namespace Microsoft.Xna.Framework.Net
{
    public class GamerLeftEventArgs : EventArgs
    {
        private NetworkGamer gamer;
        public NetworkGamer Gamer
        {
            get { return gamer; }
        }

        public GamerLeftEventArgs(NetworkGamer gamer)
        {
            this.gamer = gamer;
        }
    }
}
