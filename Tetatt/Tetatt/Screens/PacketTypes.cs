using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.Screens
{
    /// <summary>
    /// The different types of packets sent in the game.
    /// </summary>
    /// <remarks>Frequently used in packets to identify their type.</remarks>
    public enum PacketTypes
    {
        StartPlayfield,
        StopPlayfield,
        PlayerData,
        PlayerInput,
        Garbage,
    }
}
