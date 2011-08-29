using System;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsDevice
    {
        public GraphicsAdapter Adapter { get; private set; }
        public GraphicsProfile GraphicsProfile { get; private set; }
        public PresentationParameters PresentationParameters { get; private set; }
        public Viewport Viewport { get; private set; }

        public Rectangle ScissorRectangle
        {
            set {
                // TODO: This should probably use the actual viewport, and not the window viewport
                var w = Viewport.windowRectangle;
                var s = Viewport.windowScale;
                GL.Scissor(
                    (int)(w.X + s * value.Left),
                    (int)(w.Height + w.Y - s * value.Bottom), // Yay for inverted OpenGL
                    (int)(s * value.Width),
                    (int)(s * value.Height));
            }
        }

        public GraphicsDevice(GraphicsAdapter adapter,
                              GraphicsProfile graphicsProfile,
                              PresentationParameters presentationParameters)
        {
            Adapter = adapter;
            GraphicsProfile = graphicsProfile;
            PresentationParameters = presentationParameters;
            Viewport = new Viewport(presentationParameters);
        }

        public static void Clear(Color color)
        {
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void SetVertexBuffer(VertexBuffer buffer)
        {
            buffer._Activate();
        }

        static readonly BeginMode[] glPrimitiveTypes = new BeginMode[4]
        {
            BeginMode.Triangles,
            BeginMode.TriangleStrip,
            BeginMode.Lines,
            BeginMode.LineStrip
        };
        static readonly Func<int,int>[] glPrimitiveTriangleCount = new Func<int,int>[4]
        {
            c => 3*c,
            c => c+2,
            c => 2*c,
            c => c+1
        };

        public void DrawPrimitives(PrimitiveType type, int first, int count)
        {
            GL.DrawArrays(glPrimitiveTypes[(int)type], first, glPrimitiveTriangleCount[(int)type](count));
        }

        public void Present()
        {
        }
    }
}
