using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Tetatt.GamePlay;

namespace Tetatt.Graphics
{
    public class EffMoveBlock : DrawableGameComponent
    {
        private int duration;
        private int step;
        private Vector2 pos;
        private Block block;
        private SpriteBatch spriteBatch;

        public EffMoveBlock(Game game, Block block, Vector2 pos, bool left)
            : base(game)
        {
            this.pos = pos;
            duration = 5;
            this.block = block;
            step = DrawablePlayField.blockSize / duration;
            if (left)
                step = -step;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            pos.X += step;

            duration--;
            if (duration <= 0)
            {
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                DrawablePlayField.blocksTileSet.Texture,
                pos,
                DrawablePlayField.blocksTileSet.SourceRectangle(block.Tile),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

