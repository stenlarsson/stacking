using System;
using Microsoft.Xna.Framework;
using Tao.Sdl;
using Tao.OpenGl;
using Tao.DevIl;

namespace Microsoft.Xna.Framework.Graphics
{
    public enum PrimitiveType
    {
        TriangleList,
        TriangleStrip,
        LineList,
        LineStrip
    }

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
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Gl.glDisable(Gl.GL_CULL_FACE);
		}

		public static void Clear(Color color)
		{
			Gl.glClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
		}

        public void SetVertexBuffer(VertexBuffer buffer)
        {
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, buffer.buffer);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
        }

        private static readonly int[] glPrimitiveTypes = { Gl.GL_TRIANGLES, Gl.GL_TRIANGLE_STRIP, Gl.GL_LINES, Gl.GL_LINE_STRIP };
        private static readonly Func<int,int>[] glPrimitiveTriangleCount = new Func<int,int>[4]{ c => 3*c, c => c+2, c => 2*c, c => c+1 };

        public void DrawPrimitives(PrimitiveType type, int first, int count)
        {
            Gl.glDrawArrays(glPrimitiveTypes[(int)type], first, glPrimitiveTriangleCount[(int)type](count));
        }

        public void Present()
        {
            int error = Gl.glGetError();

            while (error != Gl.GL_NO_ERROR)
            {
                Console.WriteLine("OpenGL error {0}", error);
                error = Gl.glGetError();
            }

            Sdl.SDL_GL_SwapBuffers();
        }
    }
}
