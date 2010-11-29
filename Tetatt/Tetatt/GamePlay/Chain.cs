using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    class Chain
    {
        private SortedDictionary<int,Block> blocks;
        public int numBlocks {
            get {
                return blocks.Count;
            }
        }
        public int length;
        public int activeBlocks;
        public bool sentCombo;
        public bool usedThisFrame;
        public List<GarbageInfo> garbage;
        public int popCount;

        public Chain()
        {
            blocks = new SortedDictionary<int, Block>();
            length = 0;
            activeBlocks = 0;
            sentCombo = false;
            usedThisFrame = false;
            garbage = new List<GarbageInfo>();
            popCount = 0;
        }

        public bool IsActive()
        {
            return activeBlocks > 0;
        }

        public void EndFrame()
        {
            blocks.Clear();
            usedThisFrame = false;
        }

        public void AddBlock(Block block, int blocknum)
        {
            blocks[blocknum] = block;
            if (!usedThisFrame)
            {
                // Increases chain length every frame it's involved in popping new blocks
                length++;
                usedThisFrame = true;
            }
        }

        public void AddGarbage(GarbageInfo g)
        {
            garbage.Add(g); 
           sentCombo = true;
        }

        public int PopAllAndCountEvil() {
            int any = 0, evil = 0;
            foreach( KeyValuePair<int, Block> cur in blocks )
            {
                cur.Value.Pop(any++, blocks.Count);
                if (cur.Value.Type == BlockType.Gray)
                    evil++;
            }
            return evil;
        }
        
        public int TopMostBlockIndex {
            get {
                foreach( KeyValuePair<int, Block> cur in blocks )
                    return cur.Key;
                return -1; // No block...
            }
        }        
    }
}
