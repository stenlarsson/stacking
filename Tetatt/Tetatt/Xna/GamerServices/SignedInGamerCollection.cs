namespace Microsoft.Xna.Framework.GamerServices
{
    public sealed class SignedInGamerCollection :
        GamerCollection<SignedInGamer>
    {
        public SignedInGamer this [ PlayerIndex id ]
        {
            get { return this[(int)id]; }
        }
    }
}
