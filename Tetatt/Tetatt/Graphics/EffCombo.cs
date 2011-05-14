using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Tetatt.GamePlay;

namespace Tetatt.Graphics
{
    public class EffCombo : DrawableGameComponent
    {
        private int tile;
        private int duration;
        private Vector2 pos;
        private SpriteBatch spriteBatch;
        private float radius;

        public EffCombo(Game game, Vector2 pos, bool isChain, int count, int duration)
            : base(game)
        {
            this.pos = pos;
            tile = (isChain ? 52 : 38) + count;
            this.duration = duration;
            radius = 70;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
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
                DrawablePlayField.blocksTileSet.Texture,
                pos,
                DrawablePlayField.blocksTileSet.SourceRectangle(tile),
                Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            float r = radius + DrawablePlayField.blockSize;
            int t = Math.Abs(duration - 80);
            for (int i = 0; i < 6; i++)
            {
                float theta = (t + 60 * i) * (MathHelper.Pi / 180) * 7;
                spriteBatch.Draw(
                    DrawablePlayField.blocksTileSet.Texture,
                    new Vector2(
                        (r * (float)Math.Cos(theta) + pos.X),
                        (r * (float)Math.Sin(theta) + pos.Y)),
                    DrawablePlayField.blocksTileSet.SourceRectangle(91),
                    Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}