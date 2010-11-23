using System;
#if WINDOWS || XBOX
#else
namespace Microsoft.Xna.Framework
{
	namespace Graphics {
		public struct Color {
			public Color(byte r, byte g, byte b, byte a) {
				R = r; G = g; B = b; A = a;
			}
			public readonly byte R, G, B, A;
			public static Color Black { get { return new Color(0,0,0,255); } }
			public static Color White { get { return new Color(255,255,255,255); } }
			public static Color DarkGray { get { return new Color(169,169,169,255); } }
		}
		public enum SpriteSortMode { Deferred }
		public class BlendState {}
		public class SamplerState {}
		public class DepthStencilState {}
		public class RasterizerState {
			public bool ScissorTestEnable { get; set; }
		}
	}
	namespace Audio {}
	namespace GamerServices {}
	namespace Media {}

	public class GameTime {}
	public enum PlayerIndex { One, Two, Three, Four }
	public struct Vector2 {
		public Vector2(float x, float y) {
			X = x; Y = y;
		}
		public float X, Y;
	}
}
#endif