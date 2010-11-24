using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public struct Color
	{
		public Color(byte r, byte g, byte b, byte a)
		{
			R = r; G = g; B = b; A = a;
		}
		
		public readonly byte R, G, B, A;
		
		public static Color Black { get { return new Color(0,0,0,255); } }
		
		public static Color White { get { return new Color(255,255,255,255); } }
		
		public static Color DarkGray { get { return new Color(169,169,169,255); } }
	}
}
