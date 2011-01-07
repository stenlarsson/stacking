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
				Gl.glEnable(Gl.GL_SCISSOR_TEST);
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
			Draw(texture, position, null, color);
		}
		
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
			Rectangle src = sourceRectangle.HasValue ? (Rectangle)sourceRectangle : texture.Bounds;
			Draw(texture, new Rectangle((int)position.X, (int)position.Y, src.Width, src.Height), src, color);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
			Gl.glColor4ub(color.R, color.G, color.B, color.A);
			
			Rectangle dest = destinationRectangle;
			Rectangle src = sourceRectangle.HasValue ? (Rectangle)sourceRectangle : texture.Bounds;
			
			Gl.glMatrixMode(Gl.GL_TEXTURE);
			Gl.glLoadIdentity();
			Gl.glTranslatef(0, 1, 0);
			Gl.glScalef(1.0f/texture.Width, -1.0f/texture.Height, 1);

			Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture.id);
			Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
			Gl.glTexCoord2d(src.Left, src.Top);
			Gl.glVertex2i(dest.Left, dest.Top);
			Gl.glTexCoord2d(src.Left, src.Bottom);
			Gl.glVertex2i(dest.Left, dest.Bottom);
			Gl.glTexCoord2d(src.Right, src.Top);
			Gl.glVertex2i(dest.Right, dest.Top);
			Gl.glTexCoord2d(src.Right, src.Bottom);
			Gl.glVertex2i(dest.Right, dest.Bottom);
			Gl.glEnd();
		}

        public void DrawString (SpriteFont font, string text, Vector2 position, Color color)
        {
            foreach (char c in text)
            {
                Draw(font.texture, position, font.chars[c], color);
                position.X += font.chars[c].Width + font.Spacing;
            }
        }

	}
}
