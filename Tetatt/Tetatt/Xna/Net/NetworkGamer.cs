using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net
{
	public class NetworkGamer : Gamer
	{
        internal protected NetworkGamer(string gamertag, byte id) : base(gamertag)
        {
            Id = id;
        }

        public bool IsReady { get; set; }
        public bool IsMutedByLocalUser { get { return true; } }
        public bool HasVoice { get { return false; } }
        public bool IsTalking { get { return false; } }
        public bool IsLocal { get { return true; } }
        public byte Id { get; private set; }
	}
}

