using System;

namespace Microsoft.Xna.Framework.Net
{
    [Flags]
    public enum SendDataOptions
    {
        None = 0,
        Reliable,
        InOrder,
        ReliableInOrder = Reliable | InOrder,
        Chat,
    }
}

