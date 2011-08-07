using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed class EffectPassCollection : IEnumerable<EffectPass>
    {
        EffectPass pass;

        internal EffectPassCollection(BasicEffect effect)
        {
            this.pass = new EffectPass(effect);
        }

        public IEnumerator<EffectPass> GetEnumerator()
        {
            yield return pass;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
