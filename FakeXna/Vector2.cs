using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework
{
    [Serializable,StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct Vector2
    {
        public float X, Y;

        public Vector2(float v)
        {
            X = v;
            Y = v;
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static Vector2 operator+(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator-(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator*(Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 operator/(Vector2 a, float b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }

        public static Vector2 Zero { get { return new Vector2(0); } }
        public static Vector2 One { get { return new Vector2(1); } }
    }
}
