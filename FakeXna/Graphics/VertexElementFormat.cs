using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public enum VertexElementFormat
    {
        Color,
        Vector2,
        Vector3
    }

    internal static class _VertexElementFormatExtension
    {
        public static int _ComponentCount(this VertexElementFormat format)
        {
            switch (format)
            {
            case VertexElementFormat.Vector2:
                return 2;
            case VertexElementFormat.Vector3:
                return 3;
            case VertexElementFormat.Color:
                return 4;
            default:
                throw new NotImplementedException();
            }
        }
    }
}
