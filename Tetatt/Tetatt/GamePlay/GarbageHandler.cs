using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    public class GarbageHandler
    {
        class GBInfo
        {
            public BigGarbageBlock block;
            public Chain chain;

            public GBInfo(BigGarbageBlock block, Chain chain)
            {
                this.block = block;
                this.chain = chain;
            }
        }

        /** Blocks that a currently in play. */
        private List<BigGarbageBlock> activeBlocks;
        /** Blocks waiting to be dropped into play. */
        private Queue<BigGarbageBlock> normalDrops;
        private Queue<BigGarbageBlock> chainDrops;
        /** Additional information about blocks being popped. */
        private LinkedList<GBInfo> popBlocks;

        public GarbageHandler()
        {
            activeBlocks = new List<BigGarbageBlock>();
            normalDrops = new Queue<BigGarbageBlock>();
            chainDrops = new Queue<BigGarbageBlock>();
            popBlocks = new LinkedList<GBInfo>();
        }

        public void AddGarbage(int num, GarbageType type)
        {
	        switch(type)
	        {
	        case GarbageType.Chain:
		        chainDrops.Enqueue(new BigGarbageBlock(num, GarbageType.Chain));
		        break;
	        case GarbageType.Combo:
		        {
			        int rows = (num - PlayField.width + 1) / PlayField.width;
			        int small = num - rows * PlayField.width;
			        if(small >= 7) {
				        int half = small / 2; // Divide into (3,4) (4,4) (4,5) (5,5)
				        normalDrops.Enqueue(new BigGarbageBlock(half, GarbageType.Combo));
				        normalDrops.Enqueue(new BigGarbageBlock(small-half, GarbageType.Combo));
			        }
			        else
				        normalDrops.Enqueue(new BigGarbageBlock(small, GarbageType.Combo));
			        while(rows-- > 0)
				        normalDrops.Enqueue(new BigGarbageBlock(PlayField.width, GarbageType.Combo));
		        }
		        break;
	        case GarbageType.Evil:
		        while(num-- > 0)
			        normalDrops.Enqueue(new BigGarbageBlock(0, GarbageType.Evil));
		        break;
	        }
        }

        bool DropGarbageHelper(Queue<BigGarbageBlock> drops, List<BigGarbageBlock> active, PlayField pf)
        {
	        while(drops.Count != 0)
	        {
                BigGarbageBlock gb = drops.Dequeue();

		        // Abort if we cannot place all garbage.
		        if (!pf.InsertGarbage(gb))
			        return false;

		        gb.SetGraphic();
		
		        active.Add(gb);
	        }

	        return true;
        }

        public void DropGarbage(PlayField pf)
        {
	        if(normalDrops.Count == 0 && chainDrops.Count == 0)
		        return;
	
	        // Process all garbage blocks about to drop.
	        if (DropGarbageHelper(normalDrops, activeBlocks, pf))
		        DropGarbageHelper(chainDrops, activeBlocks, pf);
        }

        public void Update()
        {
            foreach(BigGarbageBlock block in activeBlocks)
                block.Update();

            activeBlocks.RemoveAll(block => block.IsEmpty());
        }

        public void AddPop(BigGarbageBlock newPop, Chain chain, bool first)
        {
	        GBInfo info = new GBInfo(newPop, chain);
	        if (first)
		        popBlocks.AddFirst(info);
	        else
		        popBlocks.AddLast(info);
        }

        public void Pop(int popStartOffset, int popTime, int flashTime)
        {
            int numBlocks = 0;
            foreach (GBInfo info in popBlocks)
                numBlocks += info.block.GetNum();

            int delay = 0;
            foreach (GBInfo info in popBlocks)
            {
                info.block.Pop(delay, numBlocks, info.chain, popStartOffset, popTime, flashTime);
                delay += info.block.GetNum();
            }

            popBlocks.Clear();
        }
    }
}
