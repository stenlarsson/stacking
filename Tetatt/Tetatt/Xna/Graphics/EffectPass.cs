using System;
using OpenTK.Graphics.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed class EffectPass
    {
        BasicEffect effect;

        internal EffectPass(BasicEffect effect)
        {
            this.effect = effect;
        }

        public void Apply()
        {
            GL.MatrixMode(MatrixMode.Projection);
            effect.Projection.glLoadMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
            effect.World.glLoadMatrix();
            effect.View.glMultMatrix();

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
}
