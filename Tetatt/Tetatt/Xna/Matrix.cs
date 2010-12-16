using System;
using System.Collections.Generic;
using Tao.OpenGl;
namespace Microsoft.Xna.Framework
{
    [Serializable]
    public struct Matrix : IEquatable<Matrix>
    {
        public float
            M11, M12, M13, M14,
            M21, M22, M23, M24,
            M31, M32, M33, M34,
            M41, M42, M43, M44;

        public Matrix (
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }

        public static Matrix Identity {
            get {
                return new Matrix(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);
            }
        }

        public bool Equals (Matrix other)
        {
            return Equals((ValueType)other);
        }

        public static Matrix CreateOrthographicOffCenter(
            float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
            float[] m = new float[16];
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glOrtho(left, right, bottom, top, zNearPlane, zFarPlane);
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, m);
            Console.WriteLine("Ortho {0}", String.Join(",", Array.ConvertAll(m, f => f.ToString())));
            return new Matrix(
                m[0],m[1],m[2],m[3],m[4],m[5],m[6],m[7],m[8],m[9],m[10],m[11],m[12],m[13],m[14],m[15]);
        }

        internal float[] ToArray()
        {
            return new float[16] {
                M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44
            };
        }
    }
}

