using System;
using System.Collections;
using System.Collections.Generic;
using Tao.OpenGl;
namespace Microsoft.Xna.Framework.Graphics
{
    public sealed class EffectPassCollection : IEnumerable<EffectPass>
    {
        private EffectPass pass;

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

    public sealed class EffectTechnique
    {
        internal EffectPassCollection passes;

        internal EffectTechnique(BasicEffect effect)
        {
            this.passes = new EffectPassCollection(effect);
        }

        public EffectPassCollection Passes {
            get {
                return passes;
            }
        }
    }

    public class Effect : GraphicsResource
    {
        public Effect(GraphicsDevice device, byte[] effectCode) : base(device)
        {
        }

        public EffectTechnique CurrentTechnique { get; set; }
    }

    public interface IEffectMatrices
    {
    }

    public interface IEffectLights
    {
    }

    public interface IEffectFog
    {
    }

    public sealed class EffectPass
    {
        private BasicEffect effect;

        internal EffectPass(BasicEffect effect)
        {
            this.effect = effect;
        }

        public void Apply()
        {
            Matrix m = effect.Projection;
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadMatrixf(m.ToArray());

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            m = effect.World;
            Gl.glLoadMatrixf(m.ToArray());
            m = effect.View;
            Gl.glMultMatrixf(m.ToArray());

            if (effect.VertexColorEnabled)
            {
                 Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
            }
            else
            {
                 Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);
            }
        }
    }

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

