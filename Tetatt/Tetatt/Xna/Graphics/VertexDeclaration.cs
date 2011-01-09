using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public class VertexDeclaration
    {
        private VertexElement[] elems;

        private int stride;
        public int VertexStride
        {
            get { return stride; }
        }

        public VertexDeclaration(int stride, params VertexElement[] elems)
        {
            this.stride = stride;
            this.elems = elems;
        }

        public VertexElement[] GetVertexElements()
        {
            return elems;
        }
    }
}
