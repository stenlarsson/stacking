using System;

namespace Tetatt.GamePlay
{
    public class PoppedEventArgs : EventArgs
    {
        public Pos pos;
        public bool isGarabge;
        public Chain chain;

        public PoppedEventArgs(Pos pos, bool isGarabge, Chain chain)
        {
            this.pos = pos;
            this.isGarabge = isGarabge;
            this.chain = chain;
        }
    }
}
