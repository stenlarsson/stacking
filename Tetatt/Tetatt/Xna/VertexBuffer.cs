using System;
using System.Runtime.InteropServices;
using Tao.OpenGl;
namespace Microsoft.Xna.Framework.Graphics
{
    public abstract class GraphicsResource : IDisposable
    {
        public GraphicsDevice GraphicsDevice { get { return device; } }
        private GraphicsDevice device;

        public GraphicsResource(GraphicsDevice device)
        {
            this.device = device;
        }

        ~GraphicsResource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
        }
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct VertexPositionColor
    {
        public VertexPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }

        Vector3 Position;
        Color Color;
    }

    [Flags]
    public enum BufferUsage
    {
        None, WriteOnly
    }

    public class VertexBuffer : GraphicsResource
    {
        public BufferUsage BufferUsage { get { return usage; } }
        private BufferUsage usage;

        public int VertexCount { get { return count; } }
        private int count;

        internal uint buffer;
        internal Type type;

        public VertexBuffer(GraphicsDevice device, Type type, int count, BufferUsage usage) : base(device)
        {
            this.usage = usage;
            this.count = count;
            this.type = type;

            Gl.glGenBuffersARB(1, out this.buffer);
        }

        public void SetData<T>(T[] data) where T : struct
        {
            int size = Marshal.SizeOf(type);
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, buffer);
            // FIXME: Don't use hard-coded values here...
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, new IntPtr(count * size), data, Gl.GL_STATIC_DRAW);
            Gl.glVertexPointer(3, Gl.GL_FLOAT, size, IntPtr.Zero);
            Gl.glColorPointer(4, Gl.GL_UNSIGNED_BYTE, size, new IntPtr(3*sizeof(float)));
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
        }
    }
}

