using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tetatt.GamePlay;

namespace Tetatt.Screens
{
    class Player
    {
        public static Vector2[] Offsets = new Vector2[] {
            new Vector2(96, 248),
            new Vector2(384, 248),
            new Vector2(672, 248),
            new Vector2(960, 248),
        };

        public int Wins;
        public int StartLevel;
        public PlayerIndex Index;
        public Vector2 Offset;
        public PlayField PlayField;

        public Player(PlayerIndex index)
        {
            Wins = 0;
            StartLevel = 4;
            Index = index;
            Offset = Offsets[(int)index];
            PlayField = null;
        }
    }
}
