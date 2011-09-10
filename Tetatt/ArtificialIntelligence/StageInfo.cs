using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.ArtificialIntelligence
{
    class StageInfo
    {
        public readonly int InputDelay;
        public readonly int RaiseHeight;
        public readonly int RaiseHeightWithoutGarbage;
        public readonly float ChainMultiplier;

        public StageInfo(
            int inputDelay,
            int raiseHeight,
            int raiseHeightWithoutGarbage,
            float chainMultiplier)
        {
            InputDelay = inputDelay;
            RaiseHeight = raiseHeight;
            RaiseHeightWithoutGarbage = raiseHeightWithoutGarbage;
            ChainMultiplier = chainMultiplier;
        }
    }
}
