using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    public struct GarbageInfo
    {
        public GarbageInfo(int size, GarbageType type)
        {
            this.size = size;
            this.type = type;
        }

        public int size;
        public GarbageType type;
    }
}
