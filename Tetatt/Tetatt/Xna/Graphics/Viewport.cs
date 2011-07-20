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
            set { bounds = value; glViewport(); }
        }
        public int X { get { return bounds.X; } }
        public int Y { get { return bounds.Y; } }
        public int Width { get { return bounds.Width; } }
        public int Height { get { return bounds.Height; } }

        private PresentationParameters presentationParameters;
        internal Viewport(PresentationParameters presentationParameters)
        {
            this.presentationParameters = presentationParameters;
            int[] values = new int[4];
            GL.GetInteger(GetPName.Viewport, values);
            this.windowRectangle = new Rectangle(values[0], values[1], values[2], values[3]);
            this.bounds = presentationParameters.Bounds;
        }

        private float scale;
        internal float windowScale { get { return scale; } }
        private Rectangle window;
        internal Rectangle windowRectangle {
            get { return window; }
            set {
                // Use the new window rectangle, but shrunken to maintain aspect ratio
                var pb = presentationParameters.Bounds;
                scale = Math.Min(
                     (float)value.Width / pb.Width,
                     (float)value.Height / pb.Height);
                window.Width = (int)(scale * pb.Width);
                window.Height = (int)(scale * pb.Height);
                window.X = (value.Width - window.Width) / 2;
                window.Y = (value.Height - window.Height) / 2;

                glViewport();
            }
        }

        private void glViewport()
        {
            GL.Viewport(
                (int)(scale * bounds.X + window.X),
                (int)(scale * bounds.Y + window.Y),
                (int)(scale * bounds.Width),
                (int)(scale * bounds.Height));
        }
    }
}
