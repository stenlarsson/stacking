using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net
{
	public class NetworkGamer : Gamer
	{
        public bool IsReady { get; set; }
        public bool IsMutedByLocalUser { get { return false; } }
        public bool HasVoice { get { return false; } }
        public bool IsTalking { get { return false; } }
        public bool IsLocal { get { throw new NotImplementedException(); } }
        public byte Id { get { throw new NotImplementedException(); } }
	}
}

