using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    public class DrawableGameComponent : GameComponent, IDrawable
    {
        public GraphicsDevice GraphicsDevice
        {
            get { return Game.GraphicsDevice; }
        }

        public DrawableGameComponent(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            LoadContent();
        }

        protected virtual void LoadContent()
        {
        }

        protected virtual void UnloadContent()
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        bool visible = true;
        public bool Visible
        {
            get { return visible; }
            set {
                visible = value;
                OnVisibleChanged(this, EventArgs.Empty);
            }
        }
        protected virtual void OnVisibleChanged(Object sender, EventArgs args)
        {
            if (VisibleChanged != null)
                VisibleChanged(sender, args);
        }
        public event EventHandler<EventArgs> VisibleChanged;

        int drawOrder;
        public int DrawOrder
        {
            get { return drawOrder; }
            set {
                drawOrder = value;
                OnDrawOrderChanged(this, EventArgs.Empty);
            }
        }
        protected virtual void OnDrawOrderChanged(Object sender, EventArgs args)
        {
            if (DrawOrderChanged != null)
                DrawOrderChanged(sender, args);
        }
        public event EventHandler<EventArgs> DrawOrderChanged;
    }
}

