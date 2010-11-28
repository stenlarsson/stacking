using System;
#if WINDOWS || XBOX
#else
namespace Microsoft.Xna.Framework
{
	namespace Graphics {
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
}
#endif