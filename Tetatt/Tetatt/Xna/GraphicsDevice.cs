using System;
using Microsoft.Xna.Framework;
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
		public Rectangle ScissorRectangle { get; set; }
		
		public GraphicsDevice(GraphicsAdapter adapter,
		                      GraphicsProfile graphicsProfile,
		                      PresentationParameters presentationParameters)
		{
			this.adapter = adapter;
			this.graphicsProfile = graphicsProfile;
			this.presentationParameters = presentationParameters;
			
			Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DOUBLEBUFFER, 1);
			
			Sdl.SDL_SetVideoMode(presentationParameters.BackBufferWidth,
			                     presentationParameters.BackBufferHeight,
			                     0, // int bpp
			                     Sdl.SDL_OPENGL);
			Gl.glViewport(0,
			              0,
			              presentationParameters.BackBufferWidth,
			              presentationParameters.BackBufferHeight);
			
			Ilut.ilutRenderer(Ilut.ILUT_OPENGL);

			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(Gl.GL_CULL_FACE);
		}
		
		public static void Clear(Color color)
		{
			Gl.glClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
		}
		
		public void Present()
		{
			Sdl.SDL_GL_SwapBuffers();
		}
	}
}
