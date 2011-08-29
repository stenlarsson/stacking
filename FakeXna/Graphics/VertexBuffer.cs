using System;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public class VertexBuffer : GraphicsResource
    {
        public BufferUsage BufferUsage { get; private set; }
        public int VertexCount { get; private set; }

        internal uint buffer;
        VertexDeclaration decl;

        public VertexBuffer(GraphicsDevice device, Type type, int count, BufferUsage usage)
            : this(device, ((IVertexType)Activator.CreateInstance(type)).VertexDeclaration, count, usage)
        {
        }

        public VertexBuffer(GraphicsDevice device, VertexDeclaration decl, int count, BufferUsage usage)
            : base(device)
        {
            BufferUsage = usage;
            VertexCount = count;
            this.decl = decl;

            if (usage != BufferUsage.None)
                throw new NotImplementedException();

            GL.GenBuffers(1, out buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(count * decl.VertexStride), IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferSubData<T>(
                BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(Math.Min(VertexCount, data.Length) * decl.VertexStride), data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        internal void _Activate()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            int size = decl.VertexStride;
            foreach (VertexElement e in decl.GetVertexElements())
            {
                switch(e.ElementUsage)
                {
                case VertexElementUsage.Color:
                    if (e.ElementFormat != VertexElementFormat.Color)
                        throw new NotSupportedException();
                    GL.ColorPointer(4, ColorPointerType.UnsignedByte, size, e._Offset);
                    break;
                case VertexElementUsage.Position:
                    GL.VertexPointer(e._ComponentCount, VertexPointerType.Float, size, e._Offset);
                    break;
                case VertexElementUsage.TextureCoordinate:
                    GL.TexCoordPointer(e._ComponentCount, TexCoordPointerType.Float, size, e._Offset);
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            GL.EnableClientState(ArrayCap.VertexArray);
        }
    }
}

