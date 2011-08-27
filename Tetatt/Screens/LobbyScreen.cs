using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetatt.GamePlay;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace Tetatt.Screens
{
    class LobbyScreen : GameScreen
    {
        private GameplayScreen gameplayScreen;
        private NetworkSession networkSession;

        public LobbyScreen(GameplayScreen gameplayScreen, NetworkSession networkSession)
        {
            this.gameplayScreen = gameplayScreen;
            this.networkSession = networkSession;

            // set the transition time
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PlayerIndex playerIndex;

            // Check if anyone paused the game
            if (input.IsPauseGame(null, out playerIndex))
            {
                gameplayScreen.ShowPauseScreen(playerIndex);
                return;
            }

            // Check if anyone wants to join
            if (input.IsMenuSelect(null, out playerIndex))
            {
                LocalNetworkGamer gamer = FindGamer(playerIndex);
                if (gamer == null)
                {
                    AddLocalPlayer(playerIndex);
                }
            }

            // Check if anyone wants to start the game
            if (input.IsMenuToggle(null, out playerIndex))
            {
                LocalNetworkGamer gamer = FindGamer(playerIndex);
                if (gamer != null)
                {
                   gamer.IsReady = !gamer.IsReady;
                }
            }

            // Check if anyone want to start with a higher level
            if (input.IsMenuUp(null, out playerIndex))
            {
                LocalNetworkGamer gamer = FindGamer(playerIndex);
                if (gamer != null)
                {
                    Player data = (Player)gamer.Tag;
                    data.StartLevel = (byte)Math.Min(data.StartLevel + 1, 8);
                    gameplayScreen.SendPlayerData(gamer);
                }
            }

            // Check if anyone want to start with a lower level
            if (input.IsMenuDown(null, out playerIndex))
            {
                LocalNetworkGamer gamer = FindGamer(playerIndex);
                if (gamer != null)
                {
                    Player data = (Player)gamer.Tag;
                    data.StartLevel = (byte)Math.Max(data.StartLevel - 1, 0);
                    gameplayScreen.SendPlayerData(gamer);
                }
            }
        }

        /// <summary>
        /// Find gamer by player index
        /// </summary>
        private LocalNetworkGamer FindGamer(PlayerIndex playerIndex)
        {
            foreach (var gamer in networkSession.LocalGamers)
            {
                if (gamer.SignedInGamer.PlayerIndex == playerIndex)
                {
                    return gamer;
                }
            }
            return null;
        }

        /// <summary>
        /// Add local player to the current game.
        /// </summary>
        public void AddLocalPlayer(PlayerIndex playerIndex)
        {
            if (networkSession.AllGamers.Count == networkSession.MaxGamers)
            {
                // Game full
                return;
            }

            // Join network game. Requires signed-in gamer.
            SignedInGamer gamer;
            gamer = Gamer.SignedInGamers[playerIndex];
            if (gamer == null)
            {
                // Show only online-profiles if playing over Live
                Guide.ShowSignIn(4, networkSession.SessionType == NetworkSessionType.PlayerMatch);
                return;
            }

            // TODO any exceptions possible?
            networkSession.AddLocalGamer(gamer);
        }

        /// <summary>
        /// Draws the lobby screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = ScreenManager.Font;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            foreach (var gamer in networkSession.AllGamers)
            {
                Player data = (Player)gamer.Tag;
                Vector2 offset = GameplayScreen.Offsets[networkSession.AllGamers.IndexOf(gamer)];

                string wins = data.Wins.ToString();
                spriteBatch.DrawString(
                    font,
                    Resources.Wins,
                    new Vector2(16, 16) + offset,
                    Color.White);
                spriteBatch.DrawString(
                    font,
                    wins,
                    new Vector2(168 - font.MeasureString(wins).X, 16) + offset,
                    Color.White);

                string level = (data.StartLevel + 1).ToString();
                spriteBatch.DrawString(
                    font,
                    Resources.Level,
                    new Vector2(16, 16 + font.LineSpacing) + offset,
                    Color.White);
                spriteBatch.DrawString(
                    font,
                    level,
                    new Vector2(168 - font.MeasureString(level).X, 16 + font.LineSpacing) + offset,
                    Color.White);

                string ready = gamer.IsReady ? Resources.IsReady : Resources.IsNotReady;
                spriteBatch.DrawString(
                    font,
                    ready,
                    new Vector2(16, 16 + font.LineSpacing * 3) + offset,
                    Color.White);

                if (gamer.IsLocal)
                {
                    spriteBatch.DrawString(
                        font,
                        Resources.ToggleReady,
                        new Vector2(16, 16 + font.LineSpacing * 4) + offset,
                        Color.White);
                }
            }

            for (int i = networkSession.AllGamers.Count; i < 4; i++)
            {
                Vector2 offset = GameplayScreen.Offsets[i];
                spriteBatch.DrawString(
                    font,
                    Resources.JoinInstruction,
                    new Vector2(16, 16) + offset,
                    Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
