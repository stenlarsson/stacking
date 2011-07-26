using System;

namespace Microsoft.Xna.Framework.Net
{
	public class NetworkSessionEndedEventArgs : EventArgs
	{
        public NetworkSessionEndReason EndReason
        {
            get { throw new NotImplementedException(); }
        }
	}
}

