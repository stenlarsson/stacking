using System.Collections.Generic;

namespace Microsoft.Xna.Framework.GamerServices
{
    public sealed class SignedInGamerCollection :
        GamerCollection<SignedInGamer>
    {
        internal SignedInGamerCollection(IList<SignedInGamer> list) : base(list)
        {
        }

        public SignedInGamer this [ PlayerIndex id ]
        {
            get { return this[(int)id]; }
        }
    }
}
