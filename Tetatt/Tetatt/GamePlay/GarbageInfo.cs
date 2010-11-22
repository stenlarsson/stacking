using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    struct GarbageInfo
    {
        public GarbageInfo(int size, GarbageType type)
        {
            this.size = size;
            this.type = type;
        }

        int size;
        GarbageType type;
    }
}
