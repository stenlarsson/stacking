using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework.Graphics
{
    public class Texture2D
    {
        int id;

        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Bounds
        {
            get { return new Rectangle(0, 0, Width, Height); }
        }

        Texture2D(GraphicsDevice device, Bitmap bmp)
        {
            // TODO: Support more than one graphics device...

            id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            BitmapData bmp_data = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                bmp_data.Width, 
                bmp_data.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmp_data.Scan0);

            Width = bmp_data.Width;
            Height = bmp_data.Height;

            bmp.UnlockBits(bmp_data);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        internal static Texture2D _FromPngStream(Stream stream, System.Drawing.Color? transparent = null)
        {
            Bitmap bmp = new Bitmap(stream);

            if (transparent.HasValue)
                bmp.MakeTransparent(transparent.Value);

            return new Texture2D(null, bmp);
        }

        public static Texture2D FromStream(GraphicsDevice device, Stream stream)
        {
            return new Texture2D(device, new Bitmap(stream));
        }

        internal void glBindTexture(TextureTarget target)
        {
            GL.BindTexture(target, id);
        }

    }
}

