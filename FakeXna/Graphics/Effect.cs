using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public class Effect : GraphicsResource
    {
        public Effect(GraphicsDevice device, byte[] effectCode)
            : base(device)
        {
        }

        public EffectTechnique CurrentTechnique { get; set; }
    }
}
