using System;

namespace Microsoft.Xna.Framework.Net
{
	public class GamerJoinedEventArgs : EventArgs
	{
        private NetworkGamer gamer;
        public NetworkGamer Gamer
        {
            get { return gamer; }
        }

        public GamerJoinedEventArgs(NetworkGamer gamer)
        {
            this.gamer = gamer;
        }
	}
}

