using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Tetatt.ArtificialIntelligence;

namespace Tetatt.Screens
{
    class StageScreen : Screen
    {
        VersusAIScreen versusAIScreen;

        public StageScreen(ScreenManager manager, VersusAIScreen versusAIScreen)
            : base(manager)
        {
            this.versusAIScreen = versusAIScreen;

            // set the transition time
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            // Check if anyone paused the game
            if (input.IsPauseGame(null, out playerIndex))
            {
                versusAIScreen.ShowPauseScreen(playerIndex);
                return;
            }

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                if (versusAIScreen.GameOver)
                {
                    if (versusAIScreen.Stage == VersusAIScreen.NumStages)
                    {
                        SignedInGamer gamer = Gamer.SignedInGamers[playerIndex];
                        string gamertag = (gamer == null) ? gamer.Gamertag : "(no name)";
                        RankingsStorage rankings = (RankingsStorage)ScreenManager.Game.Services.GetService(typeof(RankingsStorage));
                        Result result = new Result { Gamertag = gamertag, Ticks = versusAIScreen.Times.Sum() };
                        rankings.AddResult(versusAIScreen.Level, result, ControllingPlayer.Value);
                    }
                    ScreenManager.ReturnToMainMenu();
                }
                else
                {
                    versusAIScreen.Start();
                    ExitScreen();
                }
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the stage screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = ScreenManager.Font;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            // Draw stage info time
            string info = string.Format("{0}\n{1}\n\n{2}",
                Resources.VersusAI,
                versusAIScreen.Level.Name(),
                versusAIScreen.GameOver ?
                    Resources.GameOver :
                    string.Format(Resources.Stage, versusAIScreen.Stage + 1));
            spriteBatch.DrawString(
                font,
                info,
                new Vector2(100, 200),
                Color.White * TransitionAlpha);

            // Draw stage times
            Vector2 position = new Vector2(950, 200);
            int totalTime = 0;
            for (int i = 0; i < VersusAIScreen.NumStages; i++)
            {
                spriteBatch.DrawString(
                    font,
                    string.Format(Resources.Stage, i + 1),
                    position,
                    Color.White * TransitionAlpha);

                if (versusAIScreen.Stage > i)
                {
                    int time = versusAIScreen.Times[i];
                    totalTime += time;
                    spriteBatch.DrawString(
                        font,
                        FormatTime(time),
                        position + new Vector2(200, 0),
                        Color.White * TransitionAlpha);
                }

                position.Y += font.LineSpacing;
            }

            // Empty line
            position.Y += font.LineSpacing;

            // Draw total time
            spriteBatch.DrawString(
                font,
                Resources.Total,
                position,
                Color.White * TransitionAlpha);

            ;

            // Draw total time
            spriteBatch.DrawString(
                font,
                FormatTime(totalTime),
                position + new Vector2(200, 0),
                Color.White * TransitionAlpha);

            // Draw instruction
            spriteBatch.DrawString(
                font,
                Resources.PressAToContinue,
                new Vector2(100, 600),
                Color.White * TransitionAlpha);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static string FormatTime(int ticks)
        {
            return string.Format("{0}:{1:00}", ticks / (60 * 60), (ticks / 60) % 60);
        }
    }
}
