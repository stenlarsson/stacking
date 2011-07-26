using System;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class Gamer
    {
        public object Tag { get; set; }
        public string Gamertag
        {
            get { throw new NotImplementedException(); }
        }

        public IAsyncResult BeginGetProfile(
            AsyncCallback callback, Object asyncState)
        {
            throw new InvalidOperationException();
        }

        public GamerProfile EndGetProfile(IAsyncResult result)
        {
            throw new InvalidOperationException();
        }


        public static SignedInGamerCollection SignedInGamers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
