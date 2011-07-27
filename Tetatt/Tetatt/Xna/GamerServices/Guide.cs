using System;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class Guide
    {
        public static bool IsTrialMode
        {
            get { return false; }
        }

        public static bool IsVisible
        {
            get { return false; }
        }

        public static void ShowMarketplace(PlayerIndex id)
        {
        }

        public static void ShowSignIn(int paneCount, bool onlineOnly)
        {
            // TODO: Implement...
        }
    }
}
