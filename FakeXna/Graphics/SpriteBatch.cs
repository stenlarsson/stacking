using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteBatch
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        public SpriteBatch(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
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
            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

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

        void SetupColor(Color color)
        {
            GL.Color4(color.R, color.G, color.B, color.A);
        }

        void SetupTexture(Texture2D texture)
        {
            GL.MatrixMode(MatrixMode.Texture);
            GL.LoadIdentity();
            texture.glBindTexture(TextureTarget.Texture2D);
        }

        void SetupTextureSource(Texture2D texture, float x, float y, float width, float height)
        {
            SetupTexture(texture);
            GL.Scale(1.0f / texture.Width, 1.0f / texture.Height, 1);
            GL.Translate(x, y, 0);
            GL.Scale(width, height, 1);
        }

        void SetupTextureSourceRectangle(Texture2D texture, Rectangle src)
        {
            SetupTextureSource(texture, src.X, src.Y, src.Width, src.Height);
        }

        void SetupTextureSourceMaybeRectangle(Texture2D texture, Rectangle? maybeSrc)
        {
            if (maybeSrc.HasValue)
                SetupTextureSourceRectangle(texture, maybeSrc.Value);
            else
                SetupTexture(texture);
        }

        void SetupModelview()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        void SetupPositionWithRotation(float x, float y, float rotation, float ox, float oy)
        {
            GL.Translate(x, y, 0);
            GL.Rotate(180 * rotation / Math.PI, 0, 0, 1);
            GL.Translate(-ox, -oy, 0);
        }

        static void SetupSpriteEffects(SpriteEffects effects)
        {
            // TODO: Correct this once its needed
            float st = ((effects & SpriteEffects.FlipHorizontally) != 0) ? -1 : 1;
            float sv = ((effects & SpriteEffects.FlipVertically) != 0) ? -1 : 1;
            GL.Translate(0.5f*(st - 1), 0.5f*(sv - 1), 0);
            GL.Scale(st, sv, 1);
        }

        void FillSourceRectangle(Texture2D texture, Rectangle? sourceRectangle)
        {
            if (sourceRectangle.HasValue)
                FillRectangle(sourceRectangle.Value.Width, sourceRectangle.Value.Height);
            else
                FillRectangle(texture.Width, texture.Height);
        }


        void FillRectangle(float w, float h)
        {
            // TODO: Use vertex buffer or whatever...
            GL.Begin(BeginMode.TriangleStrip);
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, h);
            GL.TexCoord2(1, 0);
            GL.Vertex2(w, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex2(w, h);
            GL.End();
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            SetupColor(color);
            SetupTexture(texture);
            SetupModelview();
            GL.Translate(position.X, position.Y, 0);
            FillRectangle(texture.Width, texture.Height);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            SetupColor(color);
            SetupTextureSourceMaybeRectangle(texture, sourceRectangle);
            SetupModelview();
            GL.Translate(position.X, position.Y, 0);
            FillSourceRectangle(texture, sourceRectangle);
        }

        public void Draw(Texture2D texture, Rectangle dest, Color color)
        {
            SetupColor(color);
            SetupTexture(texture);
            SetupModelview();
            GL.Translate(dest.X, dest.Y, 0);
            FillRectangle(dest.Width, dest.Height);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            SetupColor(color);
            SetupTextureSourceMaybeRectangle(texture, sourceRectangle);
            SetupModelview();
            GL.Translate(destinationRectangle.X, destinationRectangle.Y, 0);
            FillRectangle(destinationRectangle.Width, destinationRectangle.Height);
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
            SetupColor(color);
            SetupTextureSourceMaybeRectangle(texture, sourceRectangle);
            SetupSpriteEffects(effects);

            SetupModelview();
            SetupPositionWithRotation(position.X, position.Y, rotation, origin.X, origin.Y);
            GL.Scale(scale, scale, 1);
            GL.Translate(0, 0, layerDepth); // TODO: Could we handle layerDepth like this?

            FillSourceRectangle(texture, sourceRectangle);
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
            SetupColor(color);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            // Apply position and rotation to projection so that we can use
            // the modelview matrix for string-internal positions.
            SetupPositionWithRotation(position.X, position.Y, rotation, origin.X, origin.Y);
            GL.Scale(scale, scale, 1);

            // TODO: Handle effects, which probably requires using MeasureString to figure
            // out the width and height of the string that we are drawing...

            SetupModelview();

            spriteFont._Draw(text, (texture, rect, pos) => {
                SetupTextureSourceRectangle(texture, rect);

                SetupModelview();
                GL.Translate(pos.X, pos.Y, 0);
                FillRectangle(rect.Width, rect.Height);
            });

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

        }
    }
}
