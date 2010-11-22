using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    class Chain
    {
        public Block[] blocks; // TODO should be a List?
        public int[] blockNum;
        public int numBlocks;
        public int length;
        public int activeBlocks;
        public bool sentCombo;
        public int popCount;
        public bool usedThisFrame;
        public List<GarbageInfo> garbage;

        public Chain()
        {
            blocks = new Block[100];
            blockNum = new int[100];
            numBlocks = 0;
            length = 0;
            activeBlocks = 0;
            sentCombo = false;
            popCount = 0;
            usedThisFrame = false;
            garbage = new List<GarbageInfo>();
        }

        public bool IsActive()
        {
            return activeBlocks > 0;
        }

	    public void ClearBlocks()
	    {
            for (int i = 0; i < 100; i++)
            {
                blocks[i] = null;
                blockNum[i] = -1;
            }
	    }

        public void AddBlock(Block block, int blocknum)
        {
            blocks[numBlocks] = block;
            blockNum[numBlocks] = blocknum;
            numBlocks++;
            if (!usedThisFrame)
            {
                // Increases chain length every frame it's involved in popping new blocks
                length++;
                popCount = 0;
                usedThisFrame = true;
            }
        }

        public void Sort()
	    {
		    bool changed = true;
		    while(changed)
		    {
			    changed = false;
			    for(int i = 0; i < numBlocks-1; i++)
			    {
				    if(blockNum[i+1] < blockNum[i])
				    {
                        // swap
                        int tmpNum = blockNum[i];
                        blockNum[i] = blockNum[i+1];
                        blockNum[i+1] = tmpNum;
                        Block tmpBlock = blocks[i];
                        blocks[i] = blocks[i+1];
                        blocks[i+1] = tmpBlock;
					    changed = true;
				    }
				    else if(blockNum[i] == blockNum[i+1])
				    {
					    for(int ii = i; ii < numBlocks-1; ii++)
					    {
						    blockNum[ii] = blockNum[ii+1];
						    blocks[ii] = blocks[ii+1];
					    }

					    numBlocks--;
					    blockNum[numBlocks] = -1;
					    blocks[numBlocks] = null;

					    changed = true;
				    }
			    }
		    }
	    }
    }
}
