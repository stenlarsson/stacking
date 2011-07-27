using System;
using System.Collections.Generic;

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

        internal static IList<SignedInGamer> signedInGamers = new List<SignedInGamer>();
        public static SignedInGamerCollection SignedInGamers
        {
            get {
                if (signedInGamers.Count == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        PlayerIndex index = (PlayerIndex)i;
                        signedInGamers.Add(new SignedInGamer("Player " + index, index));
                    }
                }
                return new SignedInGamerCollection(signedInGamers);
            }
        }
    }
}
