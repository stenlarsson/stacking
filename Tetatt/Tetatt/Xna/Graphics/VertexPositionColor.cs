using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
    [Serializable,StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VertexPositionColor : IVertexType
    {
        public VertexPositionColor(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }

#pragma warning disable 0414
        Vector3 Position;
        Color Color;
#pragma warning restore 0414

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionColor.VertexDeclaration; }
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            Marshal.SizeOf(typeof(VertexPositionColor)),
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(Marshal.SizeOf(typeof(Vector3)), VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );
    }
}
