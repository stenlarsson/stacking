using System;
using System.IO;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class GamerProfile
    {
        private Gamer gamer;

        internal GamerProfile(Gamer gamer)
        {
            this.gamer = gamer;
        }

        public Stream GetGamerPicture()
        {
            if (gamer.picture == null)
                return null;

            return new FileStream(gamer.picture, FileMode.Open, FileAccess.Read);
        }
    }
}
