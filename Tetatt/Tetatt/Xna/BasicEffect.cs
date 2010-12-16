using System;
using System.Collections;
using System.Collections.Generic;
using Tao.OpenGl;
namespace Microsoft.Xna.Framework.Graphics
{
    public sealed class EffectPassCollection : IEnumerable<EffectPass>
    {
        private BasicEffect effect;

        internal EffectPassCollection(BasicEffect effect)
        {
            this.effect = effect;
        }

        public IEnumerator<EffectPass> GetEnumerator()
        {
            yield return new EffectPass(effect);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    public sealed class EffectTechnique
    {
        internal BasicEffect effect;

        internal EffectTechnique(BasicEffect effect)
        {
            this.effect = effect;
        }


        public EffectPassCollection Passes {
            get {
                return new EffectPassCollection(effect);
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
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Matrix m = effect.Projection;
            Gl.glLoadMatrixf(m.ToArray());
            Gl.glLoadIdentity();
            Gl.glOrtho(0, 1280, 720, 0, -1, 1);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            m = effect.World;
            Gl.glLoadMatrixf(m.ToArray());
            m = effect.View;
            Gl.glMultMatrixf(m.ToArray());
            Gl.glLoadIdentity();

            /*
            float[] a = new float[16];
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, a);
            Console.WriteLine("modelview {0}", String.Join(",", Array.ConvertAll(a, f => f.ToString())));
            Gl.glGetFloatv(Gl.GL_PROJECTION_MATRIX, a);
            Console.WriteLine("projection {0}", String.Join(",", Array.ConvertAll(a, f => f.ToString())));
             */
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

