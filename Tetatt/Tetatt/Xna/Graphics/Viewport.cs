using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class Viewport
    {
        private Rectangle bounds;
        public Rectangle Bounds {
            get { return bounds; }
            set { bounds = value; GL.Viewport(bounds); }
        }
        public int X { get { return bounds.X; } }
        public int Y { get { return bounds.Y; } }
        public int Width { get { return bounds.Width; } }
        public int Height { get { return bounds.Height; } }

        internal Viewport()
        {
            int[] values = new int[4];
            GL.GetInteger(GetPName.Viewport, values);
            bounds = new Rectangle(values[0], values[1], values[2], values[3]);
        }
    }
}
