using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetatt.Screens
{
    class RankingsScreen : MenuScreen
    {
        RankingsStorage rankings;

        public RankingsScreen(RankingsStorage rankings, PlayerIndex player) : base(Resources.Rankings)
        {
            this.rankings = rankings;

            // set the transition time
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            rankings.BeginDiskSync(player);
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
                ExitScreen();
        }

        /// <summary>
        /// Draws the stage screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = ScreenManager.Font;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            const int columnWidth = 300, columnSep = 40;
            int width = rankings.Rankings.Count * (columnWidth + columnSep) - columnSep;
            int startX = (ScreenManager.GraphicsDevice.Viewport.Width - width)/2;

            Color normalColor = Color.White * TransitionAlpha;
            Color noResultsColor = Color.Gray * TransitionAlpha;
            Vector2 timeOffset = new Vector2(columnWidth, 0);

            foreach (var level in rankings.Rankings)
            {
                Vector2 position = new Vector2(startX, 200);
                startX += columnWidth + columnSep;

                // Draw level header (Easy, Normal, Hard)
                float scale = 1.2f;
                spriteBatch.DrawString(font, level.Key.Name, position, normalColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                position.Y += scale * font.LineSpacing;

                // Draw each gamertag adjusted left, and each time adjusted right
                foreach (var entry in level.Value)
                {
                    spriteBatch.DrawString(font, entry.Gamertag, position, normalColor);
                    string str = StageScreen.FormatTime(entry.Ticks);
                    Vector2 origin = new Vector2(font.MeasureString(str).X, 0);
                    spriteBatch.DrawString(font, str, position + timeOffset, normalColor, 0, origin, 1, SpriteEffects.None, 0);
                    position.Y += font.LineSpacing;
                }

                // Or indicate that there were no results...
                if (level.Value.Count == 0)
                    spriteBatch.DrawString(font, Resources.NoEntries, position, noResultsColor);
            }

            // Draw instruction
            spriteBatch.DrawString(
                font,
                Resources.PressAToContinue,
                new Vector2((ScreenManager.GraphicsDevice.Viewport.Width - font.MeasureString(Resources.PressAToContinue).X)/2, 600),
                Color.White * TransitionAlpha);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
