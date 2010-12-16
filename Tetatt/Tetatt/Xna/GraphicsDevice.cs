using System;
using Microsoft.Xna.Framework;
using Tao.Sdl;
using Tao.OpenGl;
using Tao.DevIl;

namespace Microsoft.Xna.Framework.Graphics
{
    public enum PrimitiveType
    {
        TriangleList = Gl.GL_TRIANGLES,
        TriangleStrip = Gl.GL_TRIANGLE_STRIP,
        LineList = Gl.GL_LINES,
        LineStrip = Gl.GL_LINE_STRIP
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

        public static int counter = 0;
        public void SetVertexBuffer(VertexBuffer buffer)
        {
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER, buffer.buffer);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            //Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
            Gl.glColorPointer(4, Gl.GL_UNSIGNED_BYTE, 16, 12);
            Gl.glVertexPointer(3, Gl.GL_FLOAT, 16, buffer.datta);
            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            Gl.glColor3f(0,0,0);
            Gl.glVertex3f(300,100,0);
            Gl.glArrayElement(1);
            Gl.glVertex3f(0,300,0);
            Gl.glEnd();
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
            //Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER, 0);
        }

        public void DrawPrimitives(PrimitiveType type, int first, int count)
        {
#if false
            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            Gl.glColor3f(0.5f,0.5f,0.5f);
            Gl.glVertex3f(0,0,0);
            Gl.glColor3f(0.5f, 0.5f, 0.5f);
            Gl.glVertex3f(1280,0,0);
            Gl.glColor3f(0.0f, 0.0f, 0.0f);
            Gl.glVertex3f(0,648,0);
            Gl.glColor3f(0.0f, 0.0f, 0.0f);
            Gl.glVertex3f(1280,640,0);
            Gl.glColor3f(0.24f, 0.24f, 0.24f);
            Gl.glVertex3f(0,720,0);
            Gl.glColor3f(0.24f, 0.24f, 0.24f);
            Gl.glVertex3f(1280,720,0);
            Gl.glEnd();
#endif
            /*
            // Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
            // Gl.glColorPointer(4, Gl.GL_UNSIGNED_BYTE, 16, 12);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glVertexPointer(3, Gl.GL_FLOAT, 16, 0);
            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            Gl.glArrayElement(0);
            Gl.glArrayElement(1);
            Gl.glArrayElement(2);
            Gl.glEnd();
//            Gl.glDrawArrays(Gl.GL_TRIANGLE_STRIP, 0, 6);
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);
            */
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
