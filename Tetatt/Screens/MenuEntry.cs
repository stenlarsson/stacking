#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetatt.Graphics;

namespace Tetatt.Screens
{
    /// <summary>
    /// Class representing a single entry in a MenuScreen, containing the
    /// string label and a Selected event. Subclasses can override the
    /// Update method to recompute the displayed text.
    /// </summary>
    class MenuEntry
    {
        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        public string Label { get; protected set; }

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        public MenuEntry(string label, EventHandler<PlayerIndexEventArgs> handler = null)
        {
            Label = label;
            if (handler != null)
                Selected += handler;
        }

        /// <summary>
        /// As an alternative to an EventHandler, the a simpler delegate can also be
        /// associated with the event.
        /// </summary>
        public delegate void PlayerIndexDelegate(PlayerIndex player);

        public MenuEntry(string label, PlayerIndexDelegate handler)
            : this(label, (sender, e) => handler(e.PlayerIndex))
        {
        }

        /// <summary>
        /// Updates the menu entry. Subclasses can override this to replace the item's label.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
        }

        internal void OnSelectEntry(PlayerIndex index)
        {
            Selected(this, new PlayerIndexEventArgs(index));
        }

    }
}
