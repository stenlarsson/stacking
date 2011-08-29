
using System;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Microsoft.Xna.Framework.Graphics
{
    [Serializable,StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VertexPositionTexture : IVertexType
    {
        public VertexPositionTexture(Vector3 position, Vector2 texcoord)
        {
            Position = position;
            TextureCoordinate = texcoord;
        }

#pragma warning disable 0414
        Vector3 Position;
        Vector2 TextureCoordinate;
#pragma warning restore 0414

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionTexture.VertexDeclaration; }
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            Marshal.SizeOf(typeof(VertexPositionTexture)),
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(Marshal.SizeOf(typeof(Vector3)), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );
    }
}
