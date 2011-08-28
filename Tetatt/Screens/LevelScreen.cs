using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tetatt.Screens
{
    class LevelScreen : MenuScreen
    {
        public LevelScreen()
            : base(Resources.Level, 0)
        {
            AddSimpleEntry(Resources.Easy, (player) => LevelMenuEntrySelected(0, player));
            AddSimpleEntry(Resources.Normal, (player) => LevelMenuEntrySelected(1, player));
            AddSimpleEntry(Resources.Hard, (player) => LevelMenuEntrySelected(2, player));
        }

        void LevelMenuEntrySelected(int level, PlayerIndex controllingPlayer)
        {
            VersusAIScreen versusAIScreen = new VersusAIScreen(level, ScreenManager);

            ScreenManager.AddScreen(versusAIScreen, controllingPlayer);
        }
    }
}
