using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetatt.Screens
{
    class StageScreen : GameScreen
    {
        VersusAIScreen versusAIScreen;

        public StageScreen(VersusAIScreen versusAIScreen)
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
                    ScreenManager.ReturnToMainMenu();
                }
                else
                {
                    versusAIScreen.Start();
                    ExitScreen();
                }
            }
        }

        /// <summary>
        /// Draws the stage screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = ScreenManager.Font;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();


            // Draw stage times
            Vector2 position = new Vector2(500, 100);
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
                        String.Format("{0}:{1:00}", time / (60 * 60), (time / 60) % 60),
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

            // Draw total time
            spriteBatch.DrawString(
                font,
                string.Format("{0}:{1:00}", totalTime / (60 * 60), (totalTime / 60) % 60),
                position + new Vector2(200, 0),
                Color.White * TransitionAlpha);

            // Draw instruction
            spriteBatch.DrawString(
                font,
                Resources.PressAToContinue,
                new Vector2(500, 600),
                Color.White * TransitionAlpha);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
