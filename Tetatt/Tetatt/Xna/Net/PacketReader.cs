using System;
using System.IO;

namespace Microsoft.Xna.Framework.Net
{
	public class PacketReader : BinaryReader
	{
        public PacketReader() : base(new MemoryStream())
        {
        }

        public int Position
        {
            get { throw new NotImplementedException(); }
        }

        public int Length
        {
            get { throw new NotImplementedException(); }
        }
	}
}

