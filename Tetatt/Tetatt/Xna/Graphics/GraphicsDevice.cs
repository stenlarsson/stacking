using System;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsDevice
    {
        private GraphicsAdapter adapter;
        public GraphicsAdapter Adapter
        {
            get { return adapter; }
        }

        private GraphicsProfile graphicsProfile;
        public GraphicsProfile GraphicsProfile
        {
            get { return graphicsProfile; }
        }

        private PresentationParameters presentationParameters;
        public PresentationParameters PresentationParameters
        {
            get { return presentationParameters; }
        }

        public Rectangle ScissorRectangle {
            set {
                GL.Scissor(
                    value.Left,
                    PresentationParameters.BackBufferHeight - value.Bottom, // Yay for inverted OpenGL
                    value.Width,
                    value.Height);
            }
        }

        public GraphicsDevice(GraphicsAdapter adapter,
                              GraphicsProfile graphicsProfile,
                              PresentationParameters presentationParameters)
        {
            this.adapter = adapter;
            this.graphicsProfile = graphicsProfile;
            this.presentationParameters = presentationParameters;
        }

        public static void Clear(Color color)
        {
            GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void SetVertexBuffer(VertexBuffer buffer)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.buffer);
            GL.EnableClientState(ArrayCap.VertexArray);
        }

        private static readonly BeginMode[] glPrimitiveTypes = new BeginMode[4]
        {
            BeginMode.Triangles,
            BeginMode.TriangleStrip,
            BeginMode.Lines,
            BeginMode.LineStrip
        };
        private static readonly Func<int,int>[] glPrimitiveTriangleCount = new Func<int,int>[4]
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
