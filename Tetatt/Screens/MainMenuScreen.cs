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
#endregion

namespace Tetatt.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        Texture2D logo, menu;
        string version;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base(Resources.MainMenu)
        {
            // Create our menu entries.
            MenuEntry versusAIMenuEntry = new MenuEntry(Resources.VersusAI);
            MenuEntry localMenuEntry = new MenuEntry(Resources.Local);
            MenuEntry liveMenuEntry = new MenuEntry(Resources.PlayerMatch);
            MenuEntry systemLinkMenuEntry = new MenuEntry(Resources.SystemLink);
            MenuEntry exitMenuEntry = new MenuEntry(Resources.Exit);

            // Hook up menu event handlers.
            versusAIMenuEntry.Selected += VersusAIMenuEntrySelected;
            localMenuEntry.Selected += LocalMenuEntrySelected;
            liveMenuEntry.Selected += LiveMenuEntrySelected;
            systemLinkMenuEntry.Selected += SystemLinkMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(versusAIMenuEntry);
            MenuEntries.Add(localMenuEntry);
            MenuEntries.Add(liveMenuEntry);
            MenuEntries.Add(systemLinkMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            ContentManager content = ScreenManager.Game.Content;
            logo = content.Load<Texture2D>("logo");
            menu = content.Load<Texture2D>("menu");
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


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            spriteBatch.Begin();

            // Draw logo
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 40);
            Color titleColor = Color.White * TransitionAlpha;

            titlePosition.X -= logo.Width / 2;
            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.Draw(logo, titlePosition, titleColor);


            Vector2 iconOffset = new Vector2(64, 150);
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                spriteBatch.Draw(menu, MenuEntries[i].Position - iconOffset, new Rectangle(128 * i, 0, 128, 128), titleColor);
            }

            // Draw copyright message
            Vector2 copyrightPosition = new Vector2(
                graphics.Viewport.Width / 2,
                graphics.Viewport.Height - 80);
            Vector2 copyrightOrigin = font.MeasureString(Resources.Copyright) / 2;
            Color copyrightColor = Color.White * TransitionAlpha;

            copyrightPosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, Resources.Copyright, copyrightPosition, copyrightColor,
                                   0, copyrightOrigin, 1.0f, SpriteEffects.None, 0);

            // Draw version
            Vector2 versionPosition = new Vector2(
                graphics.Viewport.TitleSafeArea.Right,
                graphics.Viewport.TitleSafeArea.Bottom);
            Vector2 versionOrigin = font.MeasureString(version);
            Color versionColor = Color.White * TransitionAlpha;

            versionPosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, version, versionPosition, versionColor,
                                   0, versionOrigin, 0.75f, SpriteEffects.None, 0);
            

            spriteBatch.End();
        }


        #endregion

        protected override void UpdateMenuEntryLocations()
        {
            float transitionOffset = 256 * (float)Math.Pow(TransitionPosition, 2);
            if (ScreenState == ScreenState.TransitionOff)
                transitionOffset *= 2;

            Vector2 center = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 500);

            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];
                int direction = (i - MenuEntries.Count / 2);
                Vector2 position = new Vector2(
                    transitionOffset * (0.5f + direction) + 200 * direction  + center.X, center.Y);

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
            }
        }
    }
}
