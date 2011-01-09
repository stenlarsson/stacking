using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public abstract class GraphicsResource : IDisposable
    {
        private GraphicsDevice device;
        public GraphicsDevice GraphicsDevice
        {
            get { return device; }
        }

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
}
