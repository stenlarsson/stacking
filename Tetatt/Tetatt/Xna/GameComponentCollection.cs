using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework
{
    public class GameComponentCollectionEventArgs : EventArgs
    {
        private IGameComponent component;
        public GameComponentCollectionEventArgs(IGameComponent component)
        {
            this.component = component;
        }

        public IGameComponent GameComponent {
            get {
                return component;
            }
        }
    }

    public class GameComponentCollection : Collection<IGameComponent>
    {
        public event EventHandler<GameComponentCollectionEventArgs> ComponentAdded;
        public event EventHandler<GameComponentCollectionEventArgs> ComponentRemoved;

        protected override void ClearItems()
        {
            foreach (IGameComponent component in this) {
                OnComponentRemoved(component);
            }
            base.ClearItems();
        }

        protected override void InsertItem(int index, IGameComponent component)
        {
            base.InsertItem(index, component);
            OnComponentAdded(component);
        }

        protected override void RemoveItem(int index)
        {
            OnComponentRemoved(this[index]);
            base.RemoveItem(index);
        }

        private void OnComponentAdded(IGameComponent component)
        {
            if (ComponentAdded != null)
                ComponentAdded(this, new GameComponentCollectionEventArgs(component));
        }

        private void OnComponentRemoved(IGameComponent component)
        {
            if (ComponentRemoved != null)
                ComponentRemoved(this, new GameComponentCollectionEventArgs(component));
        }
    }
}

