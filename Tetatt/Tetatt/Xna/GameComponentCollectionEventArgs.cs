using System;

namespace Microsoft.Xna.Framework
{
    public class GameComponentCollectionEventArgs : EventArgs
    {
        private IGameComponent component;
        public IGameComponent GameComponent
        {
            get { return component; }
        }

        public GameComponentCollectionEventArgs(IGameComponent component)
        {
            this.component = component;
        }
    }
}
