using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    public class BigGarbageBlock
    {
        private List<GarbageBlock> blocks;
	    private int numFalling;
	    private BlockState state;
	    private int popDelay;
	    private GarbageType type;

        public const int tileBlue = 72;
        public const int tileGray = 83;
        public const int tileTopOffset = 0;
        public const int tileMiddleOffset = 8;
        public const int tileFlash1Offset = 7;
        public const int tileFlash2Offset = 6;
        public const int tileBottomOffset = 16;
        public const int tileSingleOffset = 3;
        public const int graphicsDisabled = -1;
        public const int graphicsNone = 0;
	    public const int graphicsSingle = tileBlue + tileSingleOffset;
	    public const int graphicsTop    = tileBlue + tileTopOffset;
	    public const int graphicsMiddle = tileBlue + tileMiddleOffset;
	    public const int graphicsBottom = tileBlue + tileBottomOffset;
	    public const int graphicsEvil   = tileGray;

        public BigGarbageBlock(int num, GarbageType type, RandomBlocks randomBlocks)
        {
            blocks = new List<GarbageBlock>();
            numFalling = 0;
            state = BlockState.Idle;
            popDelay = 0;
            this.type = type;

            int lineCount = (type != GarbageType.Chain) ? 1 : num;
            int blockCount = (type == GarbageType.Combo) ? num : lineCount * PlayField.width;
            for (int i = 0; i < blockCount; i++)
            {
                blocks.Add(new GarbageBlock(type, randomBlocks.Next(0.0), this));
            }
        }

	    public int GetNum()
        {
            return blocks.Count;
        }

	    public bool IsEmpty()
        {
            return GetNum() <= 0;
        }

        public void InitPop(Chain chain)
        {
            foreach(Block block in blocks)
            {
                block.Popped = true;
            	// Needed so adjacent garbageblocks triggered by this one gets the same chain.
                block.Chain = chain;
            }
        }

        /** Get the correct graphics style specific line in a block of a certain size. */
        private int GetLineGraphic(int line, int lineCount)
        {      
          if (lineCount == 1)
		        return graphicsSingle;
	        else if (line == 0)
		        return graphicsTop;
	        else if (line == lineCount - 1)
		        return graphicsBottom;
	        else
		        return graphicsMiddle;
        }

        /** Adjust the tile number for block ends. */
        private int GetBlockGraphic(int type, int block, int blockCount)
        {
	        if(block == 0)
		        return (int)type + 0;
	        else if(block == blockCount - 1)
		        return (int)type + 2;
	        else
		        return (int)type + 1;
        }

        /**
         * Get the appropriate tile for block index given the number of lines and
         * the number of blocks in each row.
         */
        private int GetGraphic(int index, int lineCount, int blockCount)
        {
	        return GetBlockGraphic(
		        GetLineGraphic(index / PlayField.width, lineCount), index % PlayField.width, blockCount);
        }

        /**
         * Return the number of lines given the block count.
         * 3,4,5 => 1, n*6 => n
         */
        private int GetLines(int count)
        {
	        return (count + PlayField.width - 1) / PlayField.width;
        }

        public void Pop(int delay, int total, Chain newchain, int popStartOffset, int popTime, int flashTime)
        {
	        // int required here since we're stopping on less then 0
            int firstRemoved = Math.Max(blocks.Count, PlayField.width) - PlayField.width;

            for(int i = blocks.Count - 1; i >= firstRemoved; i--)
		        blocks[i].Pop(delay++, total, graphicsDisabled, popStartOffset, popTime, flashTime);

	        for(int i = firstRemoved - 1; i >= 0; i--)
		        blocks[i].Pop(
			        delay++, total,
			        GetGraphic(i, GetLines(blocks.Count) - 1, PlayField.width),
                    popStartOffset, popTime, flashTime);

	        state = BlockState.Pop;
            popDelay = popStartOffset + popTime * total + flashTime;
        }

        public void SetGraphic()
        {
	        if(type == GarbageType.Evil)
		        for(int i = 0; i < PlayField.width; i++)
			        blocks[i].SetGraphic(
				        GetBlockGraphic(graphicsEvil, i, PlayField.width));
	        else
		        for(int i = 0; i < blocks.Count; i++)
			        blocks[i].SetGraphic(
				        GetGraphic(
					        i, GetLines(blocks.Count),
					        blocks.Count < PlayField.width ? blocks.Count : PlayField.width));
        }

        public Block GetBlock(int num)
        {
	        return blocks[num];
        }

        public void Drop()
        {
	        numFalling++;
	        if((numFalling == blocks.Count || numFalling == PlayField.width) && state != BlockState.Pop)
	        {
                foreach(Block block in blocks)
                    block.State = BlockState.Falling;
		        state = BlockState.Falling;
		        numFalling = 0;
	        }
        }

        public void Land()
        {
	        if(state == BlockState.Falling)
	        {
                foreach(Block block in blocks)
                    block.State = BlockState.Idle;
		        state = BlockState.Idle;
	        }
        }

        public void Hover(int delay)
        {
            foreach(Block block in blocks)
                block.State = BlockState.Idle;
        }

        public void Update()
        {
	        if(state == BlockState.Idle && numFalling != 0)
                foreach(Block block in blocks)
                    block.State = BlockState.Idle;

	        if(state == BlockState.Pop && --popDelay <= 0)
	        {
		        state = BlockState.Falling;
		        // Clear the chain from all but the bottom blocks
		        for(int i = 0; i < (int)blocks.Count-PlayField.width; i++)
			        blocks[i].Chain = null;
	        }
	        numFalling = 0;
        }

        public void RemoveBlock()
        {
            blocks.RemoveAt(blocks.Count - 1);
        }
    }
}
