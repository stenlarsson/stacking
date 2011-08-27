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
            : base(Resources.Level)
        {
            MenuEntry easyMenuEntry = new MenuEntry(Resources.Easy);
            MenuEntry normalMenuEntry = new MenuEntry(Resources.Normal);
            MenuEntry hardMenuEntry = new MenuEntry(Resources.Hard);

            easyMenuEntry.Selected += (sender, e) => LevelMenuEntrySelected(0, e.PlayerIndex);
            normalMenuEntry.Selected += (sender, e) => LevelMenuEntrySelected(1, e.PlayerIndex);
            hardMenuEntry.Selected += (sender, e) => LevelMenuEntrySelected(2, e.PlayerIndex);

            MenuEntries.Add(easyMenuEntry);
            MenuEntries.Add(normalMenuEntry);
            MenuEntries.Add(hardMenuEntry);
        }

        void LevelMenuEntrySelected(int level, PlayerIndex controllingPlayer)
        {
            VersusAIScreen versusAIScreen = new VersusAIScreen(level, ScreenManager);

            ScreenManager.AddScreen(versusAIScreen, controllingPlayer);
        }
    }
}
