#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Tetatt.Networking;
using Microsoft.Xna.Framework.GamerServices;
using System.Collections.Generic;
#endregion

namespace Tetatt.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base(Resources.MainMenu)
        {
            // Create our menu entries.
            MenuEntry localMenuEntry = new MenuEntry(Resources.Local);
            MenuEntry liveMenuEntry = new MenuEntry(Resources.PlayerMatch);
            MenuEntry systemLinkMenuEntry = new MenuEntry(Resources.SystemLink);
            MenuEntry exitMenuEntry = new MenuEntry(Resources.Exit);

            // Hook up menu event handlers.
            localMenuEntry.Selected += LocalMenuEntrySelected;
            liveMenuEntry.Selected += LiveMenuEntrySelected;
            systemLinkMenuEntry.Selected += SystemLinkMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(localMenuEntry);
            MenuEntries.Add(liveMenuEntry);
            MenuEntries.Add(systemLinkMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Local menu entry is selected.
        /// </summary>
        void LocalMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.Local, e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the Live menu entry is selected.
        /// </summary>
        void LiveMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.PlayerMatch, e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the System Link menu entry is selected.
        /// </summary>
        void SystemLinkMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.SystemLink, e.PlayerIndex);
        }


        /// <summary>
        /// Helper method shared by the Live and System Link menu event handlers.
        /// </summary>
        void CreateOrFindSession(NetworkSessionType sessionType,
                                 PlayerIndex playerIndex)
        {
            // First, we need to make sure a suitable gamer profile is signed in.
            ProfileSignInScreen profileSignIn = new ProfileSignInScreen(sessionType);

            // Hook up an event so once the ProfileSignInScreen is happy,
            // it will activate the CreateOrFindSessionScreen.
            profileSignIn.ProfileSignedIn += delegate
            {
                GameScreen createOrFind = new CreateOrFindSessionScreen(sessionType);

                ScreenManager.AddScreen(createOrFind, playerIndex);
            };

            // Activate the ProfileSignInScreen.
            ScreenManager.AddScreen(profileSignIn, playerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            MessageBoxScreen confirmExitMessageBox =
                                    new MessageBoxScreen(Resources.ConfirmExitGame);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
