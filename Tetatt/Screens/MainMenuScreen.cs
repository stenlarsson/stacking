#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Tetatt.Networking;
using Tetatt.Graphics;
#endregion

namespace Tetatt.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        string version;
		float selectionOffset;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base(Resources.MainMenu)
        {
            AddSimpleEntry(Resources.VersusAI, VersusAIMenuEntrySelected);
            AddSimpleEntry(Resources.Local, LocalMenuEntrySelected);
            AddSimpleEntry(Resources.PlayerMatch, LiveMenuEntrySelected);
            AddSimpleEntry(Resources.SystemLink, SystemLinkMenuEntrySelected);
            AddSimpleEntry(Resources.Exit, OnCancel);

            version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            selectionOffset = SelectedEntry = 2;
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Versus AI menu entry is selected.
        /// </summary>
        void VersusAIMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new LevelScreen(), e.PlayerIndex);
        }


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

        #region Update and Draw


        protected override void DrawExtras(GameTime gameTime)
        {
            Vector2 copyrightPosition = new Vector2(Viewport.Width / 2, Viewport.Height - 80);
            Vector2 copyrightOrigin = Font.MeasureString(Resources.Copyright) / 2;
            Color copyrightColor = Color.White * TransitionAlpha;

            copyrightPosition.Y -= TransitionPower * 100;

            SpriteBatch.DrawString(Font, Resources.Copyright, copyrightPosition, copyrightColor,
                                   0, copyrightOrigin, 1.0f, SpriteEffects.None, 0);
            Vector2 versionPosition = new Vector2(
                Viewport.TitleSafeArea.Right,
                Viewport.TitleSafeArea.Bottom);
            Vector2 versionOrigin = Font.MeasureString(version);
            Color versionColor = Color.White * TransitionAlpha;

            versionPosition.Y -= TransitionPower * 100;

            SpriteBatch.DrawString(Font, version, versionPosition, versionColor,
                                   0, versionOrigin, 0.75f, SpriteEffects.None, 0);
        }


        #endregion

        protected override void DrawEntries(GameTime gameTime)
        {
            float transitionOffset = TransitionPower;
            if (ScreenState == ScreenState.TransitionOff)
                transitionOffset *= 2;

            selectionOffset = MathHelper.Lerp(selectionOffset, SelectedEntry, 0.1f);

            const int width = 640;
            int mid = MenuEntries.Count/2;
            int begin = Math.Max(0, SelectedEntry - mid);
            int end = Math.Min(MenuEntries.Count, SelectedEntry + mid + 1);
            float distanceX = (1 + transitionOffset) * 2 * (width - MenuTiles.TileSize) / MenuEntries.Count;

            float centerX = ScreenManager.GraphicsDevice.Viewport.Width / 2;

            float onleftX = Math.Min(SelectedEntry, mid) * (distanceX + transitionOffset);
            float startX = centerX - onleftX - distanceX * (selectionOffset - SelectedEntry);

            Color color = Color.White * TransitionAlpha;
            Vector2 position = new Vector2(startX, 400);
            Vector2 textOffset = new Vector2(0, 96);
            for (int i = begin; i < end; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];

                float scale = 1f - 0.35f * Math.Abs((selectionOffset-i)/(float)(mid));

                Vector2 iconOrigin = new Vector2(MenuTiles.TileSize / 2);
                SpriteBatch.Draw(
                    MenuTiles.Texture, position, MenuTiles.SourceRectangle(i), color,
                    0, iconOrigin, scale, SpriteEffects.None, 0);

                Vector2 textOrigin = Font.MeasureString(menuEntry.Label) / 2;
                Vector2 textPosition = position + textOffset * scale;
                SpriteBatch.DrawString(
                    Font, menuEntry.Label, textPosition, color * scale, 0, textOrigin,
                    scale, SpriteEffects.None, 0);

                position.X += distanceX;
            }
        }
    }
}
