using System;

namespace Microsoft.Xna.Framework
{
    public class GameTime
    {
        internal static TimeSpan totalGameTime;
        public TimeSpan TotalGameTime { get { return totalGameTime; } }

        internal TimeSpan elapsedGameTime;
        public TimeSpan ElapsedGameTime { get { return elapsedGameTime; } }

        internal GameTime(double elapsedTime)
        {
            elapsedGameTime = new TimeSpan((long)(elapsedTime * 10000000));
        }
    }
}
