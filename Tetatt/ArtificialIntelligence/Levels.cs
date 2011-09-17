using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.ArtificialIntelligence
{
    public static class Levels
    {
        public static Level[] All = { Level.Easy, Level.Normal, Level.Hard };
        public static string[] Names = { Resources.Easy, Resources.Normal, Resources.Hard };

        public static string Name(this Level level)
        {
            return Names[(int)level];
        }
    }
}
