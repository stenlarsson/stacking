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
        TimeSpan timestamp;
        int prevSelectedEntry = -1;
        Vector2 selectedPosition;

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
            timestamp = TimeSpan.Zero;
            selectedPosition = Vector2.Zero;
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

            const int width = 640;
            float distanceX = (1 + transitionOffset) * 2 * (width - MenuTiles.TileSize) / MenuEntries.Count;
            float startX =
                ScreenManager.GraphicsDevice.Viewport.Width / 2 - // Center
                (MenuEntries.Count / 2) * (distanceX + transitionOffset) + // Leftmost center
                0.5f * transitionOffset; // Force center object to transition towards right...

            Color color = Color.White * TransitionAlpha;
            Vector2 iconOffset = new Vector2(0, 96);
            Vector2 position = new Vector2(startX, 500);
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];
                bool isSelected = IsActive && SelectedEntry == i;

                Vector2 origin = Font.MeasureString(menuEntry.Label) / 2;
                SpriteBatch.DrawString(
                    Font, menuEntry.Label, position, color, 0, origin,
                    1.0f, SpriteEffects.None, 0);

                float scale = 1.0f;
                Vector2 iconPosition = position - iconOffset;
                if (isSelected)
                {
                    selectedPosition = iconPosition;
                    if (i != prevSelectedEntry)
                    {
                        prevSelectedEntry = i;
                        timestamp = gameTime.TotalGameTime;
                    }
                    scale += .18f * (float)Math.Sin(0.008*(gameTime.TotalGameTime - timestamp).TotalMilliseconds);
                }

                Vector2 iconOrigin = new Vector2(scale * MenuTiles.TileSize / 2);
                if (ScreenState == ScreenState.TransitionOff && SelectedEntry == i)
                {
                    iconPosition = new Vector2(
                        MathHelper.Lerp(selectedPosition.X, LogoPosition.X-2, TransitionPosition),
                        MathHelper.Lerp(selectedPosition.Y, LogoPosition.Y-2, TransitionPower));
                    iconOrigin = new Vector2(
                        MathHelper.Lerp(iconOrigin.X, 1, TransitionPosition),
                        MathHelper.Lerp(iconOrigin.Y, 1, TransitionPosition));
                    scale = MathHelper.Lerp(1f, 172f/(MenuTiles.TileSize - 2), TransitionPosition);
                    color = Color.White;
                }

                SpriteBatch.Draw(
                    MenuTiles.Texture, iconPosition, MenuTiles.SourceRectangle(i), color,
                    0, iconOrigin, scale, SpriteEffects.None, 0);

                position.X += distanceX;
            }
        }
    }
}
