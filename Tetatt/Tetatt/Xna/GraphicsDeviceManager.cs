using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
	public class GraphicsDeviceManager
	{
		private GraphicsDevice graphicsDevice;
		public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }
		
		public GraphicsProfile GraphicsProfile { get; set; }
		
		public int PreferredBackBufferWidth { get; set; }
		public int PreferredBackBufferHeight { get; set; }

		public GraphicsDeviceManager(Game game)
		{
			GraphicsProfile = GraphicsProfile.HiDef;
			PreferredBackBufferWidth = 1280;
			PreferredBackBufferHeight = 720;
			
			game.Services.AddService(typeof(GraphicsDeviceManager), this);
		}
		
		public bool BeginDraw()
		{
			return true;
		}
		
		public void CreateDevice()
		{
			GraphicsAdapter graphicsAdapter = new GraphicsAdapter();
			PresentationParameters presentationParameters = new PresentationParameters() {
				BackBufferHeight = PreferredBackBufferHeight,
				BackBufferWidth = PreferredBackBufferWidth
			};
			
			graphicsDevice = new GraphicsDevice(graphicsAdapter,
			                                    GraphicsProfile,
			                                    presentationParameters);
		}
		
		public void EndDraw()
		{
			graphicsDevice.Present();
		}
	}
}
