using System;

namespace Microsoft.Xna.Framework
{
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Left
        {
            get { return X; }
        }
        public int Right
        {
            get { return X + Width; }
        }
        public int Top
        {
            get { return Y; }
        }
        public int Bottom
        {
            get { return Y + Height; }
        }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        // Implicit cast to System.Drawing.Rectangle
        public static implicit operator System.Drawing.Rectangle(Rectangle r)
        {
            return new System.Drawing.Rectangle(r.X, r.Y, r.Width, r.Height);
        }
        // Implicit cast from System.Drawing.Rectangle
        public static implicit operator Rectangle(System.Drawing.Rectangle r)
        {
            return new Rectangle(r.X, r.Y, r.Width, r.Height);
        }
    }
}
