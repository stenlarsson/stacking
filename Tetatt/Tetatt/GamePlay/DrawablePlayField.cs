using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetatt.Graphics;

namespace Tetatt.GamePlay
{
    public class DrawablePlayField : DrawableGameComponent
    {
        private Vector2 offset;
        public Vector2 Offset { get { return offset; } }

        private SpriteBatch spriteBatch;

        public const int blockSize = 32;

        // TODO accessor
        public static TileSet blocksTileSet;
        public static Texture2D background;
        public static Texture2D marker;

        public readonly PlayField PlayField;

        public DrawablePlayField(Game game, Vector2 offset)
            : base(game)
        {
            this.offset = offset;
            this.PlayField = new PlayField(4);
            this.PlayField.Popped += (_, e) =>
                {
                    game.Components.Add(
                        new EffPop(game, PosToVector(e.pos)));
                };
            this.PlayField.PerformedCombo += (_, e) =>
                {
                    game.Components.Add(
                        new EffCombo(game, PosToVector(e.pos), e.isChain, e.count, PlayField.GetLevelData().effComboDuration));
                };
            this.PlayField.Swapped += (_, e) =>
                {
                    if (e.left != null)
                        game.Components.Add(new EffMoveBlock(game, e.left, PosToVector(e.pos), false));

                    if (e.right != null)
                        game.Components.Add(new EffMoveBlock(game, e.right, PosToVector(e.pos)+new Vector2(blockSize, 0), true));
                };

        }

        public override void Initialize()
        {
            PlayField.Start();

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

        public override void Update (GameTime time)
        {
            PlayField.Update();

            base.Update (time);
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
                PlayField.width * blockSize,
                PlayField.visibleHeight * blockSize);

            // Draw blocks
            PlayField.EachVisibleBlock( (row, col, block) =>
                {
                    if (block != null)
                    {
                        int tile = block.Tile;
                        Vector2 pos = PosToVector(new Pos(row,col));
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
                } );

            spriteBatch.End();

            // Draw frame and background
            spriteBatch.Begin();
            spriteBatch.Draw(
                marker,
                PosToVector(PlayField.markerPos) - new Vector2(4, 5),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 PosToVector(Pos pos)
        {
            return new Vector2(
                pos.Col * blockSize + offset.X,
                (PlayField.visibleHeight - pos.Row) * blockSize + (int)(PlayField.scrollOffset * blockSize) + offset.Y);
        }

    }
}

