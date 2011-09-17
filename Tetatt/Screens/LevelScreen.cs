using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tetatt.ArtificialIntelligence;

namespace Tetatt.Screens
{
    class LevelScreen : MenuScreen
    {
        public LevelScreen(ScreenManager manager)
            : base(manager, Resources.Level, 0)
        {
            AddSimpleEntry(Resources.Easy, (player) => LevelMenuEntrySelected(Level.Easy, player));
            AddSimpleEntry(Resources.Normal, (player) => LevelMenuEntrySelected(Level.Normal, player));
            AddSimpleEntry(Resources.Hard, (player) => LevelMenuEntrySelected(Level.Hard, player));
        }

        void LevelMenuEntrySelected(Level level, PlayerIndex controllingPlayer)
        {
            VersusAIScreen versusAIScreen = new VersusAIScreen(ScreenManager, level);

            ScreenManager.AddScreen(versusAIScreen, controllingPlayer);
        }
    }
}
