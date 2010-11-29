using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tetatt.GamePlay
{
    class PoppedEventArgs : EventArgs
    {
        public Vector2 pos;
        public bool isGarabge;
        public Chain chain;

        public PoppedEventArgs(Vector2 pos, bool isGarabge, Chain chain)
        {
            this.pos = pos;
            this.isGarabge = isGarabge;
            this.chain = chain;
        }
    }
}
