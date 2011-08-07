using System;

namespace Microsoft.Xna.Framework
{
    public interface IDrawable
    {
        int DrawOrder { get; }
        bool Visible { get; }
        void Draw(GameTime gameTime);
        event EventHandler<EventArgs> DrawOrderChanged;
        event EventHandler<EventArgs> VisibleChanged;
    }
}
