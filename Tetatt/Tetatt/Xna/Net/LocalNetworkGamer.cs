using System;
using System.Net.Sockets;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net
{
    using Packet = Tuple<byte[],NetworkGamer>;

    public sealed class LocalNetworkGamer : NetworkGamer
	{
        internal Queue<Packet> incoming = new Queue<Packet>();
        internal Queue<Packet> outgoing = new Queue<Packet>();

        internal LocalNetworkGamer(string gamertag, byte id) : base(gamertag, id)
        {
        }

        public SignedInGamer SignedInGamer
        {
            get { return new SignedInGamer(Gamertag); }
        }

        public bool IsDataAvailable
        {
            get { return incoming.Count > 0; }
        }

        public int ReceiveData(
            PacketReader data,
            out NetworkGamer sender)
        {
            if (incoming.Count == 0)
            {
                sender = null;
                return 0;
            }

            Packet packet = incoming.Dequeue();
            data.Set(packet.Item1);
            sender = packet.Item2;

            return data.Length;
        }

        public void SendData(
            PacketWriter data,
            SendDataOptions options)
        {
            SendData(data, options, null);
        }

        public void SendData(
            PacketWriter data,
            SendDataOptions options,
            NetworkGamer receiver)
        {
            outgoing.Enqueue(new Packet(data.Get(), receiver));
        }

    }
}

