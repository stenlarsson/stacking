using System;

namespace Tetatt.GamePlay
{
    public class SwapEventArgs : EventArgs
    {
        public Block left;
        public Block right;
        public Pos pos;

        public SwapEventArgs(Block left, Block right, Pos pos)
        {
            this.left = left;
            this.right = right;
            this.pos = pos;
        }
    }
}

