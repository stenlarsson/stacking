using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    public interface IGameComponent
    {
        void Initialize ();
    }

    public interface IDrawable
    {
        int DrawOrder { get; }
        bool Visible { get; }
        void Draw (GameTime gameTime);
        event EventHandler<EventArgs> DrawOrderChanged;
        event EventHandler<EventArgs> VisibleChanged;
    }

    public interface IUpdateable
    {
        int UpdateOrder { get; }
        bool Enabled { get; }
        void Update (GameTime gameTime);
        event EventHandler<EventArgs> EnabledChanged;
        event EventHandler<EventArgs> UpdateOrderChanged;
    }

    public class GameComponent : IGameComponent, IUpdateable, IDisposable
    {
        private Game game;
        public Game Game {
            get {
                return game;
            }
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

    public class DrawableGameComponent : GameComponent, IDrawable
    {
        public GraphicsDevice GraphicsDevice {
            get {
                return Game.GraphicsDevice;
            }
        }

        public DrawableGameComponent(Game game) : base(game)
        {
            Initialize(); // TODO: This is a hack. Should (probably) be done when added to Components
        }

        public override void Initialize ()
        {
            LoadContent();
        }

        protected virtual void LoadContent ()
        {
        }

        protected virtual void UnloadContent ()
        {
        }

        public virtual void Draw (GameTime gameTime)
        {
        }

        private bool visible = true;
        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
                OnVisibleChanged(this, EventArgs.Empty);
            }
        }
        protected virtual void OnVisibleChanged (Object sender,EventArgs args)
        {
            if (VisibleChanged != null)
                VisibleChanged(sender, args);
        }
        public event EventHandler<EventArgs> VisibleChanged;

        private int drawOrder;
        public int DrawOrder {
            get {
                return drawOrder;
            }
            set {
                drawOrder = value;
                OnDrawOrderChanged(this, EventArgs.Empty);
            }
        }
        protected virtual void OnDrawOrderChanged (Object sender,EventArgs args)
        {
            if (DrawOrderChanged != null)
                DrawOrderChanged(sender, args);
        }
        public event EventHandler<EventArgs> DrawOrderChanged;
    }
}

