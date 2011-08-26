using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class Viewport
    {
        Rectangle bounds;
        public Rectangle Bounds {
            get { return bounds; }
            set { bounds = value; glViewport(); }
        }
        public Rectangle TitleSafeArea {
            get { return bounds; }
        }
        public int X { get { return bounds.X; } }
        public int Y { get { return bounds.Y; } }
        public int Width { get { return bounds.Width; } }
        public int Height { get { return bounds.Height; } }


        PresentationParameters presentationParameters;
        internal Viewport(PresentationParameters presentationParameters)
        {
            this.presentationParameters = presentationParameters;
            int[] values = new int[4];
            GL.GetInteger(GetPName.Viewport, values);
            this.windowRectangle = new Rectangle(values[0], values[1], values[2], values[3]);
            this.bounds = presentationParameters.Bounds;
        }

        internal float windowScale { get; private set; }
        Rectangle window;
        internal Rectangle windowRectangle {
            get { return window; }
            set {
                // Use the new window rectangle, but shrunken to maintain aspect ratio
                var pb = presentationParameters.Bounds;
                windowScale = Math.Min(
                     (float)value.Width / pb.Width,
                     (float)value.Height / pb.Height);
                window.Width = (int)(windowScale * pb.Width);
                window.Height = (int)(windowScale * pb.Height);
                window.X = (value.Width - window.Width) / 2;
                window.Y = (value.Height - window.Height) / 2;

                glViewport();
            }
        }

        void glViewport()
        {
            GL.Viewport(
                (int)(windowScale * bounds.X + window.X),
                (int)(windowScale * bounds.Y + window.Y),
                (int)(windowScale * bounds.Width),
                (int)(windowScale * bounds.Height));
        }
    }
}
