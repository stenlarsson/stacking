using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net
{
	public class InviteAcceptedEventArgs : EventArgs
	{
        public SignedInGamer Gamer
        {
            get; private set;
        }

        public InviteAcceptedEventArgs(SignedInGamer gamer)
        {
            Gamer = gamer;
        }

        public bool IsCurrentSession
        {
            get { throw new NotImplementedException(); }
        }
	}
}

