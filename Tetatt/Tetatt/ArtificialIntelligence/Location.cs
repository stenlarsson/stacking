using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.GamePlay;
using System.Diagnostics;

namespace Tetatt.ArtificialIntelligence
{
    [DebuggerDisplay("{Type} {InChain}")]
    struct Location
    {
        public BlockType? Type;
        public bool InChain;
    }
}
