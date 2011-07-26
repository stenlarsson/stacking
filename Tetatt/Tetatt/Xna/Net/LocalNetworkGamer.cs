using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net
{
	public sealed class LocalNetworkGamer : NetworkGamer
	{
        public SignedInGamer SignedInGamer
        {
            get { return new SignedInGamer(this); }
        }

        public bool IsDataAvailable
        {
            get { throw new NotImplementedException(); }
        }

        public int ReceiveData(
            PacketReader data,
            out NetworkGamer sender)
        {
            throw new NotImplementedException();
        }

        public int SendData(
            PacketWriter data,
            SendDataOptions options)
        {
            throw new NotImplementedException();
        }

        public int SendData(
            PacketWriter data,
            SendDataOptions options,
            NetworkGamer receiver)
        {
            throw new NotImplementedException();
        }

    }
}

