using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    class GarbageHandler
    {
        class GBInfo
        {
            public GarbageBlock block;
            public Chain chain;

            public GBInfo(GarbageBlock block, Chain chain)
            {
                this.block = block;
                this.chain = chain;
            }
        }

        /** Blocks that a currently in play. */
        private List<GarbageBlock> activeBlocks;
        /** Blocks waiting to be dropped into play. */
        private Queue<GarbageBlock> normalDrops;
        private Queue<GarbageBlock> chainDrops;
        /** Additional information about blocks being popped. */
        private LinkedList<GBInfo> popBlocks;

        public GarbageHandler()
        {
            activeBlocks = new List<GarbageBlock>();
            normalDrops = new Queue<GarbageBlock>();
            chainDrops = new Queue<GarbageBlock>();
            popBlocks = new LinkedList<GBInfo>();
        }

        public void AddGarbage(int num, int player, GarbageType type)
        {
	        switch(type)
	        {
	        case GarbageType.Chain:
		        chainDrops.Enqueue(new GarbageBlock(num, GarbageType.Chain));
		        break;
	        case GarbageType.Combo:
		        {
			        int rows = (num - PlayField.width + 1) / PlayField.width;
			        int small = num - rows * PlayField.width;
			        if(small >= 7) {
				        int half = small / 2; // Divide into (3,4) (4,4) (4,5) (5,5)
				        normalDrops.Enqueue(new GarbageBlock(half, GarbageType.Combo));
				        normalDrops.Enqueue(new GarbageBlock(small-half, GarbageType.Combo));
			        }
			        else
				        normalDrops.Enqueue(new GarbageBlock(small, GarbageType.Combo));
			        while(rows-- > 0)
				        normalDrops.Enqueue(new GarbageBlock(PlayField.width, GarbageType.Combo));
		        }
		        break;
	        case GarbageType.Evil:
		        while(num-- > 0)
			        normalDrops.Enqueue(new GarbageBlock(0, GarbageType.Evil));
		        break;
	        }
        }

        bool DropGarbageHelper(Queue<GarbageBlock> drops, List<GarbageBlock> active, PlayField pf)
        {
	        while(drops.Count != 0)
	        {
                GarbageBlock gb = drops.Dequeue();

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
            foreach(GarbageBlock block in activeBlocks)
                block.Update();

            activeBlocks.RemoveAll(block => block.IsEmpty());
        }

        public void AddPop(GarbageBlock newPop, Chain chain, bool first)
        {
	        GBInfo info = new GBInfo(newPop, chain);
	        if (first)
		        popBlocks.AddFirst(info);
	        else
		        popBlocks.AddLast(info);
        }

        public void Pop()
        {
            int numBlocks = 0;
            foreach (GBInfo info in popBlocks)
                numBlocks += info.block.GetNum();

            int delay = 0;
            foreach (GBInfo info in popBlocks)
            {
                info.block.Pop(delay, numBlocks, info.chain);
                delay += info.block.GetNum();
            }

            popBlocks.Clear();
        }
    }
}
