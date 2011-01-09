using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed class EffectTechnique
    {
        internal EffectPassCollection passes;

        internal EffectTechnique(BasicEffect effect)
        {
            this.passes = new EffectPassCollection(effect);
        }

        public EffectPassCollection Passes {
            get { return passes; }
        }
    }
}
