using System;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class SignedInGamer : Gamer
    {
        internal SignedInGamer(Gamer org)
        {
            throw new NotImplementedException();
        }

        public PlayerIndex PlayerIndex
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSignedInToLive
        {
            get { throw new NotImplementedException(); }
        }

        public GamerPresence Presence
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsGuest
        {
            get { throw new NotImplementedException(); }
        }

        public GamerPrivileges Privileges
        {
            get { throw new NotImplementedException(); }
        }
    }
}
