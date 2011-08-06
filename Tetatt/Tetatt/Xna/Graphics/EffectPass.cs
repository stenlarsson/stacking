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
}
