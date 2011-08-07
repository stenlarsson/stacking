using System;
using System.IO;

namespace Microsoft.Xna.Framework.Net
{
	public class PacketReader : BinaryReader
	{
        public PacketReader() : base(new MemoryStream())
        {
        }

        internal void Set(byte[] data)
        {
            BaseStream.Position = 0;
            BaseStream.Write(data, 0, data.Length);
            BaseStream.Position = 0;
            BaseStream.SetLength(data.LongLength);
        }

        public int Position
        {
            get { return (int)BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public int Length
        {
            get { return (int)BaseStream.Length; }
        }
	}
}

