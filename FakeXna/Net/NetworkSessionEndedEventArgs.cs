using System;

namespace Microsoft.Xna.Framework.Net
{
	public class NetworkSessionEndedEventArgs : EventArgs
	{
        internal NetworkSessionEndedEventArgs(NetworkSessionEndReason reason)
        {
            EndReason = reason;
        }

        public NetworkSessionEndReason EndReason
        {
            get; private set;
        }
	}
}

