using System;

namespace Microsoft.Xna.Framework.Net
{
    public class NetworkException : Exception
    {
    }

    public class NetworkNotAvailableException : NetworkException
    {
    }

    public class NetworkSessionJoinException : NetworkException
    {
        public NetworkSessionJoinError JoinError { get; set; }
    }
}

