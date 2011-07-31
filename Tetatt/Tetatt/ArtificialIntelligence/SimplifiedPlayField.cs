using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.GamePlay;

namespace Tetatt.ArtificialIntelligence
{
    class SimplifiedPlayField
    {
        public Location[,] Field;
        public bool CanRaise;

        SimplifiedPlayField()
        {
            Field = new Location[PlayField.visibleHeight + 1, PlayField.width];
        }

        public SimplifiedPlayField(SimplifiedPlayField playField)
            : this()
        {
            for (int row = 0; row < Field.GetLength(0); row++)
            {
                for (int col = 0; col < Field.GetLength(1); col++)
                {
                    Field[row, col] = playField.Field[row, col];
                }
            }
            CanRaise = playField.CanRaise;
        }

        public SimplifiedPlayField(PlayField playField)
            : this()
        {
            playField.EachVisibleBlock((row, col, block) =>
            {
                Location loc = new Location();
                if (block != null &&
                    (block.State == BlockState.Idle ||
                    block.State == BlockState.Moving ||
                    block.State == BlockState.Hover))
                {
                    loc.Type = block.Type;
                }
                loc.Fallen = false;
                Field[row, col] = loc;
            });

            CanRaise = true;
            playField.EachVisibleBlock((row, col, block) =>
            {
                if (block != null &&
                    block.State != BlockState.Idle &&
                    block.State != BlockState.Moving)
                {
                    CanRaise = false;
                }
            });
        }

        public bool CanSwap(int row, int col)
        {
            // Cannot swap garbage, and types must be different for it to make sense
            return
                Field[row, col].Type != Field[row, col + 1].Type &&
                Field[row, col].Type != BlockType.Garbage &&
                Field[row, col + 1].Type != BlockType.Garbage;
        }

        public void Swap(int row, int col)
        {

            Location tmp = Field[row, col];
            Field[row, col] = Field[row, col + 1];
            Field[row, col + 1] = tmp;
        }

        public int Height()
        {
            int height = 0;

            for (int col = 0; col < Field.GetLength(1); col++)
            {
                for (int row = Field.GetLength(0) - 1; row >= 0 ; row--)
                {
                    if (Field[row, col].Type.HasValue)
                    {
                        height = Math.Max(height, row);
                        break;
                    }
                }
            }
            return height;
        }

        public void Settle()
        {
            // Each row except the bottom row, nothing can fall there
            for (int row = 1; row < Field.GetLength(0); row++)
            {
                // Each column except the rightmost
                for (int col = 0; col < Field.GetLength(1); col++)
                {
                    if (Field[row, col].Type.HasValue &&
                        Field[row, col].Type != BlockType.Garbage)
                    {
                        // Check each row below if something should fall
                        for (int fallRow = row; fallRow >= 1; fallRow--)
                        {
                            // If empty below
                            if (!Field[fallRow - 1, col].Type.HasValue)
                            {
                                Field[fallRow - 1, col] = Field[fallRow, col];
                                Field[fallRow - 1, col].Fallen = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
