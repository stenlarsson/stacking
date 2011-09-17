#region File Description
//-----------------------------------------------------------------------------
// JoinSessionScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using Tetatt.Screens;
#endregion

namespace Tetatt.Networking
{
    /// <summary>
    /// This menu screen displays a list of available network sessions,
    /// and lets the user choose which one to join.
    /// </summary>
    class JoinSessionScreen : MenuScreen
    {
        #region Fields

        const int MaxSearchResults = 8;

        AvailableNetworkSessionCollection availableSessions;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a menu screen listing the available network sessions.
        /// </summary>
        public JoinSessionScreen(ScreenManager manager, AvailableNetworkSessionCollection availableSessions)
            : base(manager, Resources.JoinSession)
        {
            this.availableSessions = availableSessions;

            foreach (AvailableNetworkSession availableSession in availableSessions)
            {
                // Create menu entries for each available session.
                MenuEntry menuEntry = new AvailableSessionMenuEntry(availableSession);
                menuEntry.Selected += AvailableSessionMenuEntrySelected;
                MenuEntries.Add(menuEntry);

                // Matchmaking can return up to 25 available sessions at a time, but
                // we don't have room to fit that many on the screen. In a perfect
                // world we should make the menu scroll if there are too many, but it
                // is easier to just not bother displaying more than we have room for.
                if (MenuEntries.Count >= MaxSearchResults)
                    break;
            }

            AddSimpleEntry(Resources.Back, BackMenuEntrySelected);
        }


        #endregion
           
        #region Event Handlers


        /// <summary>
        /// Event handler for when an available session menu entry is selected.
        /// </summary>
        void AvailableSessionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // Which menu entry was selected?
            AvailableSessionMenuEntry menuEntry = (AvailableSessionMenuEntry)sender;
            AvailableNetworkSession availableSession = menuEntry.AvailableSession;

            try
            {
                // Begin an asynchronous join network session operation.
                IAsyncResult asyncResult = NetworkSession.BeginJoin(availableSession,
                                                                    null, null);

                // Activate the network busy screen, which will display
                // an animation until this operation has completed.
                NetworkBusyScreen busyScreen = new NetworkBusyScreen(ScreenManager, asyncResult);

                busyScreen.OperationCompleted += JoinSessionOperationCompleted;

                ScreenManager.AddScreen(busyScreen, ControllingPlayer);
            }
            catch (Exception exception)
            {
                NetworkErrorScreen errorScreen = new NetworkErrorScreen(ScreenManager, exception);

                ScreenManager.AddScreen(errorScreen, ControllingPlayer);
            }
        }


        /// <summary>
        /// Event handler for when the asynchronous join network session
        /// operation has completed.
        /// </summary>
        void JoinSessionOperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            NetworkSession networkSession = null;
            try
            {
                // End the asynchronous join network session operation.
                networkSession = NetworkSession.EndJoin(e.AsyncResult);

                // Create a component that will manage the session we just joined.
                NetworkSessionComponent.Create(ScreenManager, networkSession);

                // Go to the gameplay screen. We pass null as the controlling player,
                // because the gameplay screen accepts input from all local players
                // who are in the session, not just a single controlling player.
                ScreenManager.AddScreen(new GameplayScreen(ScreenManager, networkSession), null);

                availableSessions.Dispose();
            }
            catch (Exception exception)
            {
                if (networkSession != null)
                    networkSession.Dispose();

                NetworkErrorScreen errorScreen = new NetworkErrorScreen(ScreenManager, exception);

                ScreenManager.AddScreen(errorScreen, ControllingPlayer);
            }
        }


        /// <summary>
        /// Event handler for when the Back menu entry is selected.
        /// </summary>
        void BackMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            availableSessions.Dispose();

            ExitScreen();
        }


        #endregion
    }
}
