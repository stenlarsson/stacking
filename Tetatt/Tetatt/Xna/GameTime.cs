using System;

namespace Microsoft.Xna.Framework
{
    public class GameTime
    {
        internal static TimeSpan totalGameTime;
        public TimeSpan TotalGameTime { get { return totalGameTime; } }

        public TimeSpan ElapsedGameTime { get; private set; }

        internal GameTime(double elapsedTime)
        {
            ElapsedGameTime = new TimeSpan((long)(elapsedTime * 10000000));
        }
    }
}
