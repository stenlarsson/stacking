using System;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class Gamer
    {
        internal string picture;

        internal Gamer(string gamertag)
        {
            Gamertag = gamertag;
        }

        public object Tag { get; set; }
        public string Gamertag
        {
            get; private set;
        }

        public IAsyncResult BeginGetProfile(AsyncCallback callback, Object asyncState)
        {
            return null;
        }

        public GamerProfile EndGetProfile(IAsyncResult result)
        {
            return new GamerProfile(this);
        }

        public static SignedInGamerCollection SignedInGamers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
