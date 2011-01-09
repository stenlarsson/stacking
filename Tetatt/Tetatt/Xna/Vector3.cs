using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework
{
    [Serializable,StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct Vector3
    {
        public float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
