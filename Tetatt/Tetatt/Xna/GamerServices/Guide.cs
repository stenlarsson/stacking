using System;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class Guide
    {
        public static bool IsTrialMode
        {
            get { return true; }
        }

        public static bool IsVisible
        {
            get { return false; }
        }

        public static void ShowMarketplace(PlayerIndex id)
        {
            throw new NotSupportedException();
        }

        public static void ShowSignIn(int paneCount, bool onlineOnly)
        {
            throw new NotSupportedException();
        }
    }
}
