using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.GamePlay;

namespace Tetatt.ArtificialIntelligence
{
    class AIPlayer
    {
        PlayField playField;
        Random random;

        public AIPlayer(PlayField playField)
        {
            this.playField = playField;
            random = new Random();
        }

        public PlayerInput GetInput()
        {
            return (PlayerInput)random.Next(7);
        }
    }
}
