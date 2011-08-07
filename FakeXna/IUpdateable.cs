using System;

namespace Microsoft.Xna.Framework
{
    public interface IUpdateable
    {
        int UpdateOrder { get; }
        bool Enabled { get; }
        void Update(GameTime gameTime);
        event EventHandler<EventArgs> EnabledChanged;
        event EventHandler<EventArgs> UpdateOrderChanged;
    }
}
