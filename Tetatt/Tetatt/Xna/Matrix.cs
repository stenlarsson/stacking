using System;
using OpenTK;

namespace Microsoft.Xna.Framework
{
    [Serializable]
    public struct Matrix
    {
        internal Matrix4 m;

        internal Matrix(Matrix4 m)
        {
            this.m = m;
        }

        public static Matrix Identity
        {
            get { return new Matrix(Matrix4.Identity); }
        }

        public bool Equals(Matrix other)
        {
            return m.Equals(other.m);
        }

        public static Matrix CreateOrthographicOffCenter(
            float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
            return new Matrix(Matrix4.CreateOrthographicOffCenter(
                left,
                right,
                bottom,
                top,
                zNearPlane,
                zFarPlane));
        }
    }
}

