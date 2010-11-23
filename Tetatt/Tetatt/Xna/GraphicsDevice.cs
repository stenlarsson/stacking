using System;
using Tao.Sdl;
using Tao.OpenGl;
using Tao.DevIl;

namespace Microsoft.Xna.Framework.Graphics
{
	public class GraphicsDevice
	{
		private GraphicsAdapter adapter;
		public GraphicsAdapter Adapter { get { return adapter; } }
		private GraphicsProfile graphicsProfile;
		public GraphicsProfile GraphicsProfile { get { return graphicsProfile; } }
		private PresentationParameters presentationParameters;
		public PresentationParameters PresentationParameters { get { return presentationParameters; } }
		public static void Clear(Color color) {}
		public Rectangle ScissorRectangle { get; set; }
		
		private IntPtr surface;
		
		public GraphicsDevice(GraphicsAdapter adapter,
		                      GraphicsProfile graphicsProfile,
		                      PresentationParameters presentationParameters)
		{
			this.adapter = adapter;
			this.graphicsProfile = graphicsProfile;
			this.presentationParameters = presentationParameters;
			
			Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DOUBLEBUFFER, 1);
			
			surface = Sdl.SDL_SetVideoMode(presentationParameters.BackBufferWidth,
			          	        		   presentationParameters.BackBufferHeight,
			                               0, // int bpp
			                               Sdl.SDL_OPENGL);
			Gl.glViewport(0,
			              0,
			              presentationParameters.BackBufferWidth,
			              presentationParameters.BackBufferHeight);
			
			Ilut.ilutRenderer(Ilut.ILUT_OPENGL);
		}
	}
}
