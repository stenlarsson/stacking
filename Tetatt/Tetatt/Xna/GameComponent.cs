
using System;
using Microsoft.Xna.Framework.Graphics;
namespace Microsoft.Xna.Framework
{
    public class GameComponent : IGameComponent, IUpdateable, IDisposable
    {
        private Game game;
        public Game Game
        {
            get { return game; }
        }

        public GameComponent(Game game)
        {
            this.game = game;
        }

        ~GameComponent() {
            Dispose(false);
        }

        private bool disposed = false;
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

        private bool enabled = true;
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

        private int updateOrder;
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
