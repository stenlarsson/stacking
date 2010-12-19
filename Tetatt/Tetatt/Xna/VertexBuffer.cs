using System;
using System.Reflection;
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

        protected virtual void Dispose(bool disposing)
        {
        }
    }

    [Serializable,StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VertexPositionColor : IVertexType
    {
        public VertexPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }

        Vector3 Position;
        Color Color;

        VertexDeclaration IVertexType.VertexDeclaration {
            get {
                return VertexDeclaration;
            }
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            Marshal.SizeOf(typeof(VertexPositionColor)),
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(Marshal.SizeOf(typeof(Vector3)), VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );


    }

    [Flags]
    public enum BufferUsage
    {
        None, WriteOnly
    }

    [Serializable]
    public struct VertexElement
    {
        public int Offset { get; set; }
        public VertexElementFormat ElementFormat { get; set; }
        public VertexElementUsage ElementUsage { get; set; }
        public int UsageIndex { get; set; }

        public VertexElement(int offset, VertexElementFormat elementFormat, VertexElementUsage elementUsage, int usageIndex)
        {
            this.Offset = offset;
            this.ElementFormat = elementFormat;
            this.ElementUsage = elementUsage;
            this.UsageIndex = usageIndex;
        }
    }

    public enum VertexElementUsage
    {
        Color, Position
    }

    public enum VertexElementFormat
    {
        Color, Vector3
    }

    public class VertexDeclaration
    {
        private VertexElement[] elems;
        private int stride;

        public VertexDeclaration(int stride, params VertexElement[] elems)
        {
            this.stride = stride;
            this.elems = elems;
        }

        public VertexElement[] GetVertexElements()
        {
            return elems;
        }

        public int VertexStride { get { return stride; } }
    }

    public interface IVertexType
    {
        VertexDeclaration VertexDeclaration { get; }
    }

    public class VertexBuffer : GraphicsResource
    {
        public BufferUsage BufferUsage { get { return usage; } }
        private BufferUsage usage;

        public int VertexCount { get { return count; } }
        private int count;

        internal uint buffer;
        internal VertexDeclaration decl;

        public VertexBuffer(GraphicsDevice device, Type type, int count, BufferUsage usage)
            : this(device, ((IVertexType)Activator.CreateInstance(type)).VertexDeclaration, count, usage)
        {
        }
        public VertexBuffer(GraphicsDevice device, VertexDeclaration decl, int count, BufferUsage usage)
            : base(device)
        {
            this.usage = usage;
            this.count = count;
            this.decl = decl;

            Gl.glGenBuffers(1, out buffer);
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing && buffer != 0)
            {
                Gl.glDeleteBuffers(1, ref buffer);
                buffer = 0;
            }
        }

        public void SetData<T>(T[] data) where T : struct
        {
            // TODO: Check that count/size agrees with the data array
            int size = decl.VertexStride;
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, buffer);
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, new IntPtr(count * size), data, Gl.GL_STATIC_DRAW);
            foreach (VertexElement e in decl.GetVertexElements())
            {
                switch(e.ElementFormat)
                {
                case VertexElementFormat.Color:
                    Gl.glColorPointer(4, Gl.GL_UNSIGNED_BYTE, size, new IntPtr(e.Offset));
                    break;
                case VertexElementFormat.Vector3:
                    Gl.glVertexPointer(3, Gl.GL_FLOAT, size, new IntPtr(e.Offset));
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
        }
    }
}

