using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    class BlockListItem : IComparable<BlockListItem>
    {
        public readonly int Index;
        public readonly Block Block;

        public BlockListItem(int index, Block block)
        {
            Index = index;
            Block = block;
        }

        public int CompareTo(BlockListItem other)
        {
            return Index - other.Index;
        }
    }
}
