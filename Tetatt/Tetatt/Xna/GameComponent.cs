
using System;
using Microsoft.Xna.Framework.Graphics;
namespace Microsoft.Xna.Framework
{
    public class GameComponent : IGameComponent, IUpdateable, IDisposable
    {
        public Game Game { get; private set; }

        public GameComponent(Game game)
        {
            Game = game;
        }

        ~GameComponent() {
            Dispose(false);
        }

        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (Disposed != null)
                    Disposed(this, EventArgs.Empty);
            }
        }
        public event EventHandler<EventArgs> Disposed;

        public virtual void Initialize ()
        {
        }

        public virtual void Update(GameTime time)
        {
        }

        bool enabled = true;
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                enabled = value;
                OnEnabledChanged(this, EventArgs.Empty);
            }
        }
        protected virtual void OnEnabledChanged (Object sender,EventArgs args)
        {
            if (EnabledChanged != null)
                EnabledChanged(sender, args);
        }
        public event EventHandler<EventArgs> EnabledChanged;

        int updateOrder;
        public int UpdateOrder {
            get {
                return updateOrder;
            }
            set {
                updateOrder = value;
                OnUpdateOrderChanged(this, EventArgs.Empty);
            }
        }
        protected virtual void OnUpdateOrderChanged (Object sender,EventArgs args)
        {
            if (UpdateOrderChanged != null)
                UpdateOrderChanged(sender, args);
        }
        public event EventHandler<EventArgs> UpdateOrderChanged;
    }
}
