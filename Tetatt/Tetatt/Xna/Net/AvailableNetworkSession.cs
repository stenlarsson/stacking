using System;

namespace Microsoft.Xna.Framework.Net
{
	public class AvailableNetworkSession
	{
        internal AvailableNetworkSession(
            int currentGamerCount,
            int openPublicGamerSlots,
            string hostGamertag,
            QualityOfService qos)
        {
            CurrentGamerCount = currentGamerCount;
            OpenPublicGamerSlots = openPublicGamerSlots;
            HostGamertag = hostGamertag;
            QualityOfService = qos;
        }

        public int CurrentGamerCount
        {
            get; private set;
        }
        public int OpenPublicGamerSlots
        {
            get; private set;
        }

        public string HostGamertag
        {
            get; private set;
        }

        public QualityOfService QualityOfService
        {
            get; private set;
        }
	}
}

