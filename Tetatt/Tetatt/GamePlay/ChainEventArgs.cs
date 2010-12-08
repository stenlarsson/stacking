using System;

namespace Tetatt.GamePlay
{
    public class ChainEventArgs : EventArgs
    {
        public Chain chain;

        public ChainEventArgs(Chain chain)
        {
            this.chain = chain;
        }
    }
}
