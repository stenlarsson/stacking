using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public class PresentationParameters
    {
        //public SurfaceFormat BackBufferFormat { get; set; }
        public int BackBufferHeight { get; set; }
        public int BackBufferWidth { get; set; }
        public Rectangle Bounds { get { return new Rectangle(0, 0, BackBufferWidth, BackBufferHeight); } }
        //public DepthFormat DepthStencilFormat { get; set; }
        //public IntPtr DeviceWindowHandle { get; set; }
        //public DisplayOrientation DisplayOrientation { get; set; }
        public bool IsFullScreen { get; set; }
        //public int MultiSampleCount { get; set; }
        //public PresentationInterval PresentationInterval { get; set; }
        //public RenderTargetUsage RenderTargetUsage { get; set; }
    }
}
