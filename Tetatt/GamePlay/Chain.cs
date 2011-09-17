using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    public class Chain
    {
        private List<BlockListItem> blocks;
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
            blocks = new List<BlockListItem>();
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
            blocks.Add(new BlockListItem(blocknum, block));
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

        public int PopAllAndCountEvil(int popStartOffset, int popTime, int flashTime) {
            int any = 0, evil = 0;
            blocks.Sort();
            foreach (var cur in blocks)
            {
                cur.Block.Pop(any++, blocks.Count, popStartOffset, popTime, flashTime);
                if (cur.Block.Type == BlockType.Gray)
                    evil++;
            }
            return evil;
        }
        
        public int TopMostBlockIndex {
            get
            {
                var top = blocks.Max();
                return top == null ? -1 : top.Index;
            }
        }        
    }
}
