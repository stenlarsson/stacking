using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetatt.GamePlay;

namespace Tetatt.Screens
{
    class LobbyScreen : GameScreen
    {
        private GameplayScreen gameplayScreen;

        public LobbyScreen(GameplayScreen gameplayScreen)
        {
            TransitionOffTime = TimeSpan.Zero;
            this.gameplayScreen = gameplayScreen;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Check if anyone paused the game
            PlayerIndex playerIndex;
            if (input.IsNewKeyPress(Keys.Escape, null, out playerIndex) ||
                input.IsNewButtonPress(Buttons.Back, null, out playerIndex))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), playerIndex);
                return;
            }

            for (int i = 0; i < InputState.MaxInputs; i++)
            {
                playerIndex = (PlayerIndex)i;
                PlayerInput? playerInput = input.GetPlayerInput(playerIndex);
                int listIndex = gameplayScreen.Players.FindIndex(p => p.Index == playerIndex);

                if (listIndex != -1)
                {
                    Player player = gameplayScreen.Players[listIndex];

                    switch(playerInput)
                    {
                        case PlayerInput.Swap:
                            gameplayScreen.StartGame();
                            ScreenManager.RemoveScreen(this);
                            break;
                        case PlayerInput.Up:
                            player.StartLevel = Math.Min(player.StartLevel + 1, 8);
                            break;
                        case PlayerInput.Down:
                            player.StartLevel = Math.Max(player.StartLevel - 1, 0);
                            break;
                    }
                }
                else
                {
                    if (playerInput == PlayerInput.Swap)
                    {
                        gameplayScreen.CreatePlayer(playerIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = ScreenManager.Font;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            foreach (var player in gameplayScreen.Players)
            {
                string wins = player.Wins.ToString();
                spriteBatch.DrawString(
                    font,
                    "Wins",
                    new Vector2(16, 16) + player.Offset,
                    Color.White);
                spriteBatch.DrawString(
                    font,
                    wins,
                    new Vector2(168 - font.MeasureString(wins).X, 16) + player.Offset,
                    Color.White);
                
                string level = (player.StartLevel + 1).ToString();
                spriteBatch.DrawString(
                    font,
                    "Level",
                    new Vector2(16, 45) + player.Offset,
                    Color.White);
                spriteBatch.DrawString(
                    font,
                    level,
                    new Vector2(168 - font.MeasureString(level).X, 45) + player.Offset,
                    Color.White);
            }

            for (int i = gameplayScreen.Players.Count; i < 4; i++)
            {
                spriteBatch.DrawString(
                    font,
                    "Press A\nto join",
                    new Vector2(16, 16) + Player.Offsets[i],
                    Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
