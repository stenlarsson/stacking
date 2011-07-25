using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tetatt.GamePlay;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Graphics;

namespace Tetatt.Screens
{
    class Player
    {
        public static byte DefaultLevel = 4;

        public int Wins;
        public int StartLevel;
        public PlayField PlayField;
        public Texture2D GamerPicture;

        public int[,] LastFieldState;
        public int SendFieldStateTimer;

        public Player()
        {
            Wins = 0;
            StartLevel = DefaultLevel;
            PlayField = null;

            LastFieldState = new int[PlayField.visibleHeight + 1, PlayField.width];
            SendFieldStateTimer = 0;
        }
    }
}
