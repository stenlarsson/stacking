using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetatt.Graphics;

namespace Tetatt.GamePlay
{
    public class DrawablePlayField : PlayField
    {
        private Vector2 offset;
        public Vector2 Offset { get { return offset; } }

        private SpriteBatch spriteBatch;

        public const int blockSize = 32;

        // TODO accessor
        public static TileSet blocksTileSet;
        public static Texture2D background;
        public static Texture2D marker;

        public DrawablePlayField(Game game, Vector2 offset)
            : base(game)
        {
            this.offset = offset;
        }

        public override void Initialize()
        {
            Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Game.Content.Load<Texture2D>("playfield");
            marker = Game.Content.Load<Texture2D>("marker");
            blocksTileSet = new TileSet(
                Game.Content.Load<Texture2D>("blocks"), blockSize);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw frame and background
            spriteBatch.Begin();
            spriteBatch.Draw(
                background,
                offset - new Vector2(16, 16), // Adjust for the frame
                Color.White);
            spriteBatch.End();

            // Setup sprite clipping using scissor test
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null,
                new RasterizerState()
                {
                    ScissorTestEnable = true
                });
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                (int)offset.X,
                (int)offset.Y,
                width * blockSize,
                visibleHeight * blockSize);

            Vector2 blocksOffset = offset + new Vector2(0, (int)(scrollOffset * blockSize));

            // Draw blocks
            for (int row = 0; row < visibleHeight+1; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Block block = field[row,col];
                    if (block != null)
                    {
                        int tile = block.Tile;
                        Vector2 pos = PosToVector(new Pos(row, col)) + blocksOffset;
                        spriteBatch.Draw(
                            blocksTileSet.Texture,
                            new Rectangle(
                                (int)pos.X,
                                (int)pos.Y,
                                blockSize,
                                blockSize),
                            blocksTileSet.SourceRectangle(tile),
                            (row == 0) ? Color.DarkGray : Color.White);
                    }
                }
            }

            spriteBatch.End();

            // Draw frame and background
            spriteBatch.Begin();
            spriteBatch.Draw(
                marker,
                PosToVector(markerPos) + blocksOffset - new Vector2(4, 5),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 PosToVector(Pos pos)
        {
            return new Vector2(
                pos.Col * blockSize,
                (visibleHeight - pos.Row) * blockSize);
        }

    }
}

