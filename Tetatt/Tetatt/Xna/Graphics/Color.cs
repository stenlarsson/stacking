using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
    [Serializable,StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct Color
    {
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r; G = g; B = b; A = a;
        }

        public Color(float r, float g, float b, float a = 1.0f)
        {
            R = (byte)(int)(255.0*r);
            G = (byte)(int)(255.0*g);
            B = (byte)(int)(255.0*b);
            A = (byte)(int)(255.0*a);
        }

        public readonly byte R, G, B, A;
        
        public static Color Black { get { return new Color(0,0,0,255); } }
        
        public static Color White { get { return new Color(255,255,255,255); } }
        
        public static Color DarkGray { get { return new Color(169,169,169,255); } }
    }
}
