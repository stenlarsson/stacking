using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.GamePlay;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Tetatt.Screens;

namespace Tetatt.Graphics
{
    /// <summary>
    /// A PlayField with added functionality to be drawn on the screen
    /// </summary>
    class DrawablePlayField : PlayField
    {
        public const int BlockSize = 32;

        public static TileSet blocksTileSet;
        public static Texture2D background;
        public static Texture2D marker;
        public static SpriteFont font;

        public Vector2 Offset;
        ScreenManager screenManager;

        /// <summary>
        /// Create a new DrawablePlayField
        /// </summary>
        public DrawablePlayField(int startLevel)
            : base(startLevel)
        {
            Offset = Vector2.Zero;
        }

        /// <summary>
        /// Draw the play field on the screen. Alpha value used to make field semi-transparent.
        /// </summary>
        public void Draw(ScreenManager screenManager, GameTime gameTime, float transitionAlpha)
        {
            // This is ugly but the screen manager might not be available during
            // the construction of this object
            this.screenManager = screenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            spriteBatch.Begin();

            Vector2 position = Offset;
            position.Y -= 32; // Adjust for the frame
            position.Y -= font.LineSpacing * 2;

            // Draw statistics
            string score = Score.ToString();
            spriteBatch.DrawString(
                font,
                Resources.Score,
                position,
                Color.White * transitionAlpha);
            spriteBatch.DrawString(
                font,
                score,
                position + new Vector2(200 - font.MeasureString(score).X, 0),
                Color.White * transitionAlpha);

            position.Y += font.LineSpacing;

            string time = String.Format("{0}:{1:00}", Time / (60 * 60), (Time / 60) % 60);
            spriteBatch.DrawString(
                font,
                Resources.Time,
                position,
                Color.White * transitionAlpha);
            spriteBatch.DrawString(
                font,
                time,
                position + new Vector2(200 - font.MeasureString(time).X, 0),
                Color.White * transitionAlpha);

            // Draw frame and background
            spriteBatch.Draw(
                background,
                Offset - new Vector2(16, 16), // Adjust for the frame
                Color.White * transitionAlpha);

            spriteBatch.End();

            // Setup sprite clipping using scissor test
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null,
                new RasterizerState()
                {
                    ScissorTestEnable = true
                });
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                (int)Offset.X,
                (int)Offset.Y,
                PlayField.width * BlockSize,
                PlayField.visibleHeight * BlockSize);

            // Draw blocks
            EachVisibleBlock((row, col, block) =>
            {
                if (block != null)
                {
                    int tile = block.Tile;
                    Vector2 pos = PosToVector(new Pos(row, col)) + Offset;
                    if (block.IsState(BlockState.Moving))
                        pos.X += (block.Right ? 1 : -1) * BlockSize * block.StateDelay / 5;

                    spriteBatch.Draw(
                        blocksTileSet.Texture,
                        new Rectangle(
                            (int)pos.X,
                            (int)pos.Y,
                            BlockSize,
                            BlockSize),
                        blocksTileSet.SourceRectangle(tile),
                        ((row == 0 || State == PlayFieldState.Dead) ? Color.DarkGray : Color.White) * transitionAlpha);
                }
            });

            spriteBatch.End();

            spriteBatch.Begin();
            if (State == PlayFieldState.Play || State == PlayFieldState.Start)
            {
                // Draw marker
                spriteBatch.Draw(
                    marker,
                    PosToVector(markerPos) + Offset - new Vector2(4, 5),
                    Color.White * transitionAlpha);
            }

            if (State == PlayFieldState.Start)
            {
                string countdown = ((StateDelay / 60) + 1).ToString();
                Vector2 size = font.MeasureString(countdown);
                spriteBatch.DrawString(
                    font,
                    countdown,
                    new Vector2(96, 96) - size / 2 + Offset,
                    Color.White * transitionAlpha);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Return screen coordinates from field position
        /// </summary>
        Vector2 PosToVector(Pos pos)
        {
            return new Vector2(
                pos.Col * BlockSize,
                (PlayField.visibleHeight - pos.Row) * BlockSize + (int)(scrollOffset * BlockSize));
        }

        /// <summary>
        /// Override ActivatePopped to draw pop effect.
        /// </summary>
        public override void ActivatePopped(Pos pos, bool isGarbage, Chain chain)
        {
            screenManager.Game.Components.Add(
               new EffPop(screenManager, PosToVector(pos) + Offset));
            
            base.ActivatePopped(pos, isGarbage, chain);
        }

        /// <summary>
        /// Override ActivatePerformedCombo to draw combo and chain panels
        /// </summary>
        public override void ActivatePerformedCombo(int pos, bool isChain, int count)
        {
            screenManager.Game.Components.Add(
                new EffCombo(screenManager, PosToVector(new Pos(pos / width, pos % width)) + Offset,
                    isChain, count,
                    GetLevelData().effComboDuration));

            base.ActivatePerformedCombo(pos, isChain, count);
        }
    }
}
