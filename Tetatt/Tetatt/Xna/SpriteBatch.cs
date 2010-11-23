using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public class SpriteBatch {
		private GraphicsDevice graphicsDevice;
		
		public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }
		
		public SpriteBatch(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
		}
		public void Begin ()
		{
		}
		
		public void Begin (SpriteSortMode sortMode,
		                   BlendState blendState = null,
		                   SamplerState samplerState = null,
		                   DepthStencilState depthStencilState = null,
		                   RasterizerState rasterizerState = null)
		{
		}
		
		public void End ()
		{
		}
		
		public void Draw (Texture2D texture, Vector2 position, Color color)
		{
		}
		
		public void Draw (Texture2D texture, Rectangle destinationRectangle, Nullable<Rectangle> sourceRectangle, Color color)
		{
		}
	}
}
