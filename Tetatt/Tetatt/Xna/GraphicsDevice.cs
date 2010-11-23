using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public class GraphicsDevice
	{
		public static void Clear(Color color) {}
		public Rectangle ScissorRectangle { get; set; }
		
		public GraphicsDevice(GraphicsAdapter adapter,
		                      GraphicsProfile graphicsProfile,
		                      PresentationParameters presentationParameters)
		{
		}
	}
}
