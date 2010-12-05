using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tetatt.GamePlay
{
    public class ComboEventArgs : EventArgs
    {
        public Vector2 pos;
        public bool isChain;
        public int count;

        public ComboEventArgs(Vector2 pos, bool isChain, int count)
        {
            this.pos = pos;
            this.isChain = isChain;
            this.count = count;
        }
    }
}
