using System;

namespace Microsoft.Xna.Framework
{
    public static class MathHelper
    {
        public const float Pi = (float)Math.PI;

        public static float Clamp(float val, float min, float max)
        {
            if (val <= min) {
                return min;
            }
            else if (val >= max) {
                return max;
            }
            else {
                return val;
            }
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
    }
}

