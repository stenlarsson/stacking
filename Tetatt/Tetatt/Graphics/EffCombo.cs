using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetatt.GamePlay;
using Tetatt.Screens;

namespace Tetatt.Graphics
{
    public class EffCombo : DrawableGameComponent
    {
        private int tile;
        private int duration;
        private Vector2 pos;
        private SpriteBatch spriteBatch;
        private float radius;

        public EffCombo(ScreenManager screenManager, Vector2 pos, bool isChain, int count, int duration)
            : base(screenManager.Game)
        {
            this.spriteBatch = screenManager.SpriteBatch;
            this.pos = pos;
            tile = (isChain ? 51 : 38) + count;
            this.duration = duration;
            radius = 70;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); 
            
            pos.Y -= duration >> 5;
            radius *= 0.9f;

            duration--;
            if (duration <= 0)
            {
                Dispose();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                GameplayScreen.blocksTileSet.Texture,
                pos,
                GameplayScreen.blocksTileSet.SourceRectangle(tile),
                Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            float r = radius + GameplayScreen.blockSize;
            int t = Math.Abs(duration - 80);
            for (int i = 0; i < 6; i++)
            {
                float theta = (t + 60 * i) * (MathHelper.Pi / 180) * 7;
                spriteBatch.Draw(
                    GameplayScreen.blocksTileSet.Texture,
                    new Vector2(
                        (r * (float)Math.Cos(theta) + pos.X),
                        (r * (float)Math.Sin(theta) + pos.Y)),
                    GameplayScreen.blocksTileSet.SourceRectangle(91),
                    Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}