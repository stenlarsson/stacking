using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    [Flags]
    public enum BlockState
    {
        Idle     = 1 << 0,
        Falling  = 1 << 1,
        Moving   = 1 << 2,
        PostMove = 1 << 3,
        Hover    = 1 << 4,
        Flash    = 1 << 5,
        Pop      = 1 << 6,
        Pop2     = 1 << 7,
        Pop3     = 1 << 8,
        Dead     = 1 << 9
    }
}
