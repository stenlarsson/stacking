using System;
using Tao.DevIl;

namespace Microsoft.Xna.Framework.Graphics
{
	public class Texture2D
	{
		private int texture;
		
		public int Width { get; set; }
		public int Height { get; set; }
		public Rectangle Bounds { get { return new Rectangle(0, 0, Width, Height); } }
		
		public Texture2D(string filename)
		{
			int image = Il.ilGenImage();
			Il.ilBindImage(image);
			if(!Il.ilLoadImage(filename))
			{
				throw new Exception(string.Format("Couldn't load image ({0}): {1}", Il.ilGetError(), filename));
			}
			Width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
			Height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT); 
			texture = Ilut.ilutGLBindTexImage();
			Il.ilDeleteImage(image);
		}
	}
}

