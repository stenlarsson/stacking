using System;

namespace Tetatt.GamePlay
{
    public class ComboEventArgs : EventArgs
    {
        public Pos pos;
        public bool isChain;
        public int count;

        public ComboEventArgs(Pos pos, bool isChain, int count)
        {
            this.pos = pos;
            this.isChain = isChain;
            this.count = count;
        }
    }
}
