using System;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class SignedInGamer : Gamer
    {
        internal SignedInGamer(string gamertag, PlayerIndex index) : base(gamertag)
        {
            Presence = new GamerPresence();
            PlayerIndex = index;
        }

        public PlayerIndex PlayerIndex
        {
            get; internal set;
        }

        public bool IsSignedInToLive
        {
            get { return false; }
        }

        public GamerPresence Presence
        {
            get; private set;
        }

        public bool IsGuest
        {
            get { return true; }
        }

        public GamerPrivileges Privileges
        {
            get { return new GamerPrivileges(); }
        }
    }
}
