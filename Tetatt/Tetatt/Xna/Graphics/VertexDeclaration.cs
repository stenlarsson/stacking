using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public class VertexDeclaration
    {
        VertexElement[] elems;

        public int VertexStride { get; private set; }

        public VertexDeclaration(int stride, params VertexElement[] elems)
        {
            VertexStride = stride;
            this.elems = elems;
        }

        public VertexElement[] GetVertexElements()
        {
            return elems;
        }
    }
}
