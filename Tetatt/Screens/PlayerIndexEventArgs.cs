#region File Description
//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;

namespace Tetatt.Screens
{
    /// <summary>
    /// Custom event argument which includes the index of the player who
    /// triggered the event. This is used by the MenuEntry.Selected event.
    /// </summary>
    class PlayerIndexEventArgs : EventArgs
    {
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public PlayerIndex PlayerIndex { get; private set; }
    }
}
