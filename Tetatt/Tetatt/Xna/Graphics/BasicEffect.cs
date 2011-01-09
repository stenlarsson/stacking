using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public class BasicEffect : Effect, IEffectMatrices, IEffectLights, IEffectFog
    {
        public Matrix World { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public bool VertexColorEnabled { get; set; }

        public BasicEffect(GraphicsDevice device) : base(device, null)
        {
            this.CurrentTechnique = new EffectTechnique(this);
        }
    }
}

