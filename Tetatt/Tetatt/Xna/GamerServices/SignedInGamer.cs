using System;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class SignedInGamer : Gamer
    {
        internal SignedInGamer(string gamertag) : base(gamertag)
        {
            Presence = new GamerPresence();
        }

        public PlayerIndex PlayerIndex
        {
            get { throw new NotImplementedException(); }
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
