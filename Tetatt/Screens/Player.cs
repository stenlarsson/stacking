using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tetatt.GamePlay;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Graphics;
using Tetatt.Graphics;

namespace Tetatt.Screens
{
    /// <summary>
    /// The data associated with a player in a multiplayer game.
    /// It is stored in the NetworkGamer.Tag property.
    /// </summary>
    class Player
    {
        /// <summary>
        /// Default starting level (range 0-8)
        /// </summary>
        public const int DefaultLevel = 4;

        /// <summary>
        /// Number of won games
        /// </summary>
        public int Wins;
        /// <summary>
        /// Chosen starting level (range 0-8)
        /// </summary>
        public int StartLevel;
        /// <summary>
        /// Playfield for this player
        /// </summary>
        public DrawablePlayField PlayField;
        /// <summary>
        /// GamerPicture of this player, or null.
        /// </summary>
        public Texture2D GamerPicture;

        /// <summary>
        /// Number of frames until input should be broadcasted for this player.
        /// </summary>
        public int SendInputTimer;
        /// <summary>
        /// The input for this player on certain frames. For local players this
        /// is the input to send, and for remote players it is the input to
        /// process. Tuples with (frame, input).
        /// </summary>
        public Queue<InputQueueItem> InputQueue;
        /// <summary>
        /// The garbage received by this player on certain frames. This is used
        /// for both local and remote players. Tuples with (frame, size, type).
        /// </summary>
        public Queue<GarbageQueueItem> GarbageQueue;

        /// <summary>
        /// Create new data with default values.
        /// </summary>
        public Player()
        {
            Wins = 0;
            StartLevel = DefaultLevel;
            PlayField = null;
            GamerPicture = null;

            SendInputTimer = 0;
            InputQueue = new Queue<InputQueueItem>();
            GarbageQueue = new Queue<GarbageQueueItem>();
        }
    }
}
