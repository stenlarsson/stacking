using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteBatch
    {
        private GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

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

            GL.PushMatrix();
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            Matrix4 mat = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
            GL.LoadMatrix(ref mat);

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Fog);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            if(rasterizerState != null && rasterizerState.ScissorTestEnable)
                GL.Enable(EnableCap.ScissorTest);

        }

        public void End ()
        {
            GL.PopAttrib();
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
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

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Draw(texture, destinationRectangle, null, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            GL.Color4(color.R, color.G, color.B, color.A);

            Rectangle dest = destinationRectangle;
            Rectangle src = sourceRectangle.HasValue ? (Rectangle)sourceRectangle : texture.Bounds;

            GL.MatrixMode(MatrixMode.Texture);
            GL.LoadIdentity();
            GL.Scale(1.0f / texture.Width, 1.0f / texture.Height, 1);

            GL.BindTexture(TextureTarget.Texture2D, texture.id);
            GL.Begin(BeginMode.TriangleStrip);
            GL.TexCoord2(src.Left, src.Top);
            GL.Vertex2(dest.Left, dest.Top);
            GL.TexCoord2(src.Left, src.Bottom);
            GL.Vertex2(dest.Left, dest.Bottom);
            GL.TexCoord2(src.Right, src.Top);
            GL.Vertex2(dest.Right, dest.Top);
            GL.TexCoord2(src.Right, src.Bottom);
            GL.Vertex2(dest.Right, dest.Bottom);
            GL.End();
        }

        public void Draw(
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth)
        {
            // TODO: Implement properly
            Draw(texture, position, sourceRectangle, color);
        }

        public void DrawString (SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            DrawString(spriteFont, text, position, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }

        public void DrawString(
            SpriteFont spriteFont,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth)
        {
            // TODO implement rotation

            float x = position.X - scale * origin.X;
            float y = position.Y - scale * origin.Y;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    x = position.X;
                    y += spriteFont.LineSpacing * scale;
                    continue;
                }
                Rectangle rect = spriteFont.chars[c];
                Draw(spriteFont.texture,
                    new Rectangle(
                        (int)x,
                        (int)y,
                        (int)(rect.Width * scale),
                        (int)(rect.Height * scale)),
                    rect,
                    color);
                x += (rect.Width + spriteFont.Spacing) * scale;
            }
        }
    }
}
