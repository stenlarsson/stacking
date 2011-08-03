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
                if (block != null)
                {
                    Field[row, col].Type = block.Type;
                    Field[row, col].InChain = (block.Chain != null);
                }
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
            // Cannot move garbage
            bool isGarbage = Field[row, col].Type == BlockType.Garbage ||
                Field[row, col + 1].Type == BlockType.Garbage;
            // Types must be different for it to make sense
            bool isDifferent = Field[row, col].Type != Field[row, col + 1].Type;
            // Cannot swap when something is about to fall down from above
            bool isBlocked = Field[row, col].Type == null && Field[row + 1, col].Type != null ||
                Field[row, col + 1].Type == null && Field[row + 1, col + 1].Type != null;

            return !isGarbage && isDifferent && !isBlocked;
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
                                Field[fallRow, col] = new Location();
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

        public void Pop()
        {
            // Each row except bottom which cannot pop
            for (int row = 1; row < Field.GetLength(0); row++)
            {
                for (int col = 0; col < Field.GetLength(1); col++)
                {
                    // TODO If there are 4 in a row we will get another PopScore in
                    // next iteration, resulting in PopScore*2 for 4 pieces. If both
                    // this and the below test are true it will result in
                    // PopScore*2 for 5 pieces...

                    if (col < Field.GetLength(1) - 2 &&
                        Field[row, col].Type.HasValue &&
                        Field[row, col].Type == Field[row, col + 1].Type &&
                        Field[row, col].Type == Field[row, col + 2].Type)
                    {
                        SetChain(row, col);
                        Field[row, col].Type = null;
                        SetChain(row, col + 1);
                        Field[row, col + 1].Type = null;
                        SetChain(row, col + 2);
                        Field[row, col + 2].Type = null;
                    }

                    if (row < Field.GetLength(0) - 2 &&
                        Field[row, col].Type.HasValue &&
                        Field[row, col].Type == Field[row + 1, col].Type &&
                        Field[row, col].Type == Field[row + 2, col].Type)
                    {
                        SetChain(row + 2, col);
                        Field[row, col].Type = null;
                        Field[row + 1, col].Type = null;
                        Field[row + 2, col].Type = null;
                    }
                }
            }
        }

        void SetChain(int row, int col)
        {
            for (int y = row + 1; y < Field.GetLength(0) && Field[y, col].Type != null; y++)
            {
                Field[y, col].InChain = true;
            }
        }
    }
}
