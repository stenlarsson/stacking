using System;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
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

            GL.GenBuffers(1, out buffer);
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing && buffer != 0)
            {
                GL.DeleteBuffers(1, ref buffer);
                buffer = 0;
            }
        }

        public void SetData<T>(T[] data) where T : struct
        {
            // TODO: Check that count/size agrees with the data array
            int size = decl.VertexStride;
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData<T>(BufferTarget.ArrayBuffer, new IntPtr(count * size), data, BufferUsageHint.StaticDraw);
            foreach (VertexElement e in decl.GetVertexElements())
            {
                switch(e.ElementFormat)
                {
                case VertexElementFormat.Color:
                    GL.ColorPointer(4, ColorPointerType.UnsignedByte, size, new IntPtr(e.Offset));
                    break;
                case VertexElementFormat.Vector3:
                    GL.VertexPointer(3, VertexPointerType.Float, size, new IntPtr(e.Offset));
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}

