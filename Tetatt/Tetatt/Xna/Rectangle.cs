using System;

namespace Microsoft.Xna.Framework
{
	public struct Rectangle
	{
		public int X;
		public int Y;
		public int Width;
		public int Height;
		
		public int Left { get { return X; } }
		public int Right { get { return X + Width; } }
		public int Top { get { return Y; } }
		public int Bottom { get { return Y + Height; } }
		
		public Rectangle(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}
	}
}
