using System;
using Tao.OpenGl;

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
			Begin(SpriteSortMode.Deferred);
		}
		
		public void Begin (SpriteSortMode sortMode,
		                   BlendState blendState = null,
		                   SamplerState samplerState = null,
		                   DepthStencilState depthStencilState = null,
		                   RasterizerState rasterizerState = null)
		{
			int width = graphicsDevice.PresentationParameters.BackBufferWidth;
			int height = graphicsDevice.PresentationParameters.BackBufferHeight;
			
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			Glu.gluOrtho2D(0, width, height, 0);
			
			Gl.glPushAttrib(Gl.GL_ALL_ATTRIB_BITS);
			
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_FOG);
			Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL);
			
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			
			if(rasterizerState != null && rasterizerState.ScissorTestEnable)
			{
				Rectangle srect = graphicsDevice.ScissorRectangle;
				Gl.glEnable(Gl.GL_SCISSOR_TEST);
				Gl.glScissor(srect.X,
				             height - srect.Height - srect.Y,
				             srect.Width,
				             srect.Height);
			}
		}
		
		public void End ()
		{
			Gl.glPopAttrib();
			Gl.glPopMatrix();
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glPopMatrix();
		}
		
		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			Draw(texture, new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height), null, color);
		}
		
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
			Gl.glColor4ub(color.R, color.G, color.B, color.A);
			
			Rectangle dest = destinationRectangle;
			Rectangle src = sourceRectangle.HasValue ? (Rectangle)sourceRectangle : texture.Bounds;
			
			double tx1 = src.Left / (double)texture.Width;
			double ty1 = (texture.Height - src.Top) / (double)texture.Height;
			double tx2 = src.Right / (double)texture.Width;
			double ty2 = (texture.Height - src.Bottom) / (double)texture.Height;

			Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture.id);
			Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
			Gl.glTexCoord2d(tx1,ty1);
			Gl.glVertex2i(dest.Left, dest.Top);
			Gl.glTexCoord2d(tx1,ty2);
			Gl.glVertex2i(dest.Left, dest.Bottom);
			Gl.glTexCoord2d(tx2,ty1);
			Gl.glVertex2i(dest.Right, dest.Top);
			Gl.glTexCoord2d(tx2,ty2);
			Gl.glVertex2i(dest.Right, dest.Bottom);
			Gl.glEnd();
		}
	}
}
