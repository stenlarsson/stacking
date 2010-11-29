using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    class ChainEventArgs : EventArgs
    {
        public Chain chain;

        public ChainEventArgs(Chain chain)
        {
            this.chain = chain;
        }
    }
}
