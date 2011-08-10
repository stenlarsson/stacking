﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    public class Chain
    {
        private class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                return y.CompareTo(x);
            }
        }

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
            blocks = new SortedDictionary<int, Block>(new DescendingComparer<int>());
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

        public int PopAllAndCountEvil(int popStartOffset, int popTime, int flashTime) {
            int any = 0, evil = 0;
            foreach( KeyValuePair<int, Block> cur in blocks )
            {
                cur.Value.Pop(any++, blocks.Count, popStartOffset, popTime, flashTime);
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