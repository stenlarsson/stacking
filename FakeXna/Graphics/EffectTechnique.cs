using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed class EffectTechnique
    {
        internal EffectTechnique(BasicEffect effect)
        {
            Passes = new EffectPassCollection(effect);
        }

        public EffectPassCollection Passes { get; private set; }
    }
}
