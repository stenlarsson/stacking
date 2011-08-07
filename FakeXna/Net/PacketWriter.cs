using System.IO;

namespace Microsoft.Xna.Framework.Net
{
    public class PacketWriter : BinaryWriter
    {
        public PacketWriter() : base(new MemoryStream())
        {
        }

        internal byte[] Get()
        {
            byte[] data = new byte[BaseStream.Length];
            long position = BaseStream.Position;
            BaseStream.Position = 0;
            BaseStream.Read(data, 0, data.Length);
            BaseStream.Position = position;
            return data;
        }
    }
}

