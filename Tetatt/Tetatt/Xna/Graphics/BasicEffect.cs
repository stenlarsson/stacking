using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;

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
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref m.m);

            GL.MatrixMode(MatrixMode.Modelview);
            m = effect.World;
            GL.LoadMatrix(ref m.m);
            m = effect.View;
            GL.MultMatrix(ref m.m);

            if (effect.VertexColorEnabled)
            {
                GL.EnableClientState(ArrayCap.ColorArray);
            }
            else
            {
                GL.DisableClientState(ArrayCap.ColorArray);
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

