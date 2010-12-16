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

        internal object datta;
        internal GCHandle handle;

        public void SetData<T>(T[] data) where T : struct
        {
            datta = data;
            handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Console.WriteLine("Sizes {0} {1}", count * Marshal.SizeOf(type), Marshal.SizeOf(data[0]));
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER, buffer);
            Gl.glBufferDataARB(Gl.GL_ARRAY_BUFFER, (IntPtr)(count * Marshal.SizeOf(type)), handle.AddrOfPinnedObject(), Gl.GL_STATIC_DRAW);

            /*
            float[] array = new float[24];
            Gl.glGetBufferSubData(Gl.GL_ARRAY_BUFFER, IntPtr.Zero, new IntPtr(96), array);
            Console.WriteLine("Ortho {0}", String.Join(";", Array.ConvertAll(array, f => f.ToString())));
             */
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
        }
    }
}

