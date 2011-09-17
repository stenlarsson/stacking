#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Tetatt.Graphics;

namespace Tetatt.Screens
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : Screen
    {
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        string menuTitle;
        TileSet tileSet;
        Texture2D logo;
        int iconTile;
        MenuInput prevInput, nextInput;

        protected int SelectedEntry {
            get; set;
        }

        protected TileSet MenuTiles {
            get; private set;
        }

        protected Vector2 LogoPosition {
            get { return new Vector2((Viewport.Width - logo.Width) / 2, 40); }
        }

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        public MenuScreen(ScreenManager manager, string menuTitle)
            : this(manager, menuTitle, -1, false)
        {
        }

        public MenuScreen(ScreenManager manager, string menuTitle, bool horizontal)
            : this(manager, menuTitle, -1, horizontal)
        {
        }

        public MenuScreen(ScreenManager manager, string menuTitle, int iconTile)
            : this(manager, menuTitle, iconTile, false)
        {
        }

        public MenuScreen(ScreenManager manager, string menuTitle, int iconTile, bool horizontal)
            : base(manager)
        {
            this.menuTitle = menuTitle;
            this.iconTile = iconTile;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            SelectedEntry = 0;

            prevInput = horizontal ? MenuInput.Left : MenuInput.Up;
            nextInput = horizontal ? MenuInput.Right : MenuInput.Down;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            logo = ScreenManager.Game.Content.Load<Texture2D>("logo");
            tileSet = new TileSet(
                ScreenManager.Game.Content.Load<Texture2D>("blocks"),
                DrawablePlayField.BlockSize);
            MenuTiles = new TileSet(
                ScreenManager.Game.Content.Load<Texture2D>("menu"),
                128);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            if (input.IsMenuInput(prevInput, ControllingPlayer, out playerIndex))
            {
                SelectedEntry--;

                if (SelectedEntry < 0)
                    SelectedEntry = menuEntries.Count - 1;
            }

            if (input.IsMenuInput(nextInput, ControllingPlayer, out playerIndex))
            {
                SelectedEntry++;

                if (SelectedEntry >= menuEntries.Count)
                    SelectedEntry = 0;
            }

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(SelectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }

        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        protected virtual void DrawEntries(GameTime gameTime)
        {
            float transitionOffset = 256 * TransitionPower;
            if (ScreenState == ScreenState.TransitionOff)
                transitionOffset *= 2;

            Color normal = Color.White * TransitionAlpha;
            Color selected = Color.Yellow * TransitionAlpha;

            float cursorRotation = (float)gameTime.TotalGameTime.TotalSeconds * 3;
            Vector2 cursorOrigin = new Vector2(tileSet.TileSize) / 2;

            Vector2 position = new Vector2(
                Viewport.Width / 2 - transitionOffset, 300f);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                bool isSelected = IsActive && SelectedEntry == i;

                Vector2 origin = Font.MeasureString(menuEntry.Label) / 2;
                SpriteBatch.DrawString(
                    Font, menuEntry.Label, position, isSelected ? selected : normal, 0, origin,
                    1.0f, SpriteEffects.None, 0);

                if (isSelected)
                {
                    Vector2 cursorPosition = new Vector2(position.X - origin.X - 32, position.Y);
                    SpriteBatch.Draw(
                        tileSet.Texture, cursorPosition, tileSet.SourceRectangle(91),
                        normal, cursorRotation, cursorOrigin,
                        1.0f, SpriteEffects.None, 0);
                }

                position.Y += Font.MeasureString(menuEntry.Label).Y;
            }

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            for (int i = 0; i < menuEntries.Count; i++)
                menuEntries[i].Update(gameTime);
        }

        protected SpriteBatch SpriteBatch {
            get { return ScreenManager.SpriteBatch; }
        }
        protected SpriteFont Font {
            get { return ScreenManager.Font; }
        }
        protected Viewport Viewport {
            get { return ScreenManager.GraphicsDevice.Viewport; }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            DrawGameLogo();
            DrawMenuTitle();
            DrawEntries(gameTime);
            DrawExtras(gameTime);
            SpriteBatch.End();
        }

        protected virtual void DrawGameLogo()
        {
            Vector2 position = LogoPosition;
            Color color = Color.White * TransitionAlpha;
            if (iconTile < 0 || ScreenState != ScreenState.TransitionOn)
                SpriteBatch.Draw(logo, position, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            if (iconTile >= 0 && ScreenState != ScreenState.TransitionOn)
                SpriteBatch.Draw(
                    MenuTiles.Texture, position, MenuTiles.SourceRectangle(iconTile), color,
                    0, new Vector2(2), 172f/(MenuTiles.TileSize-2), SpriteEffects.None, 0);
        }

        protected virtual void DrawMenuTitle()
        {
            Vector2 titlePosition = new Vector2(40, 100 - TransitionPower * 100);
            float titleScale = 1.5f;
            Vector2 titleOrigin = new Vector2(Font.MeasureString(menuTitle).X * titleScale, 0);
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;

            SpriteBatch.DrawString(Font, menuTitle, titlePosition, titleColor, -MathHelper.Pi/2,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
        }

        protected virtual void DrawExtras(GameTime gameTime)
        {
        }

        protected void AddSimpleEntry(string label, EventHandler<PlayerIndexEventArgs> handler)
        {
            menuEntries.Add(new MenuEntry(label, handler));
        }

        protected void AddSimpleEntry(string label, MenuEntry.PlayerIndexDelegate handler)
        {
            menuEntries.Add(new MenuEntry(label, handler));
        }
    }
}
