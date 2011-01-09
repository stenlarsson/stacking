using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Tetatt.GamePlay
{
    public class Popper
    {
        private List<Chain> chains = new List<Chain>();
        private Chain newChain = null;

        public void AddBlock(Block block, int blocknum)
        {
	        Chain addToChain = block.Chain;
	
	        if(addToChain == null)	// If the block isn't part of a chain
	        {
		        // Make a new chain unless one was already created this frame
		        if(newChain == null)
                {
			        chains.Add(newChain = new Chain());
		        }
		        addToChain = newChain;
		        block.Chain = newChain;
	        }

	        addToChain.AddBlock(block, blocknum);
        }

        public void Pop(int popStartOffset, int popTime, int flashTime)
        {
            int evil = 0;

            foreach (Chain chain in chains)
            {
                if (!chain.usedThisFrame)
                    continue;

                evil += chain.PopAllAndCountEvil(popStartOffset, popTime, flashTime);

                // Check add evil garbage
                if (evil > 0)
                {
                    chain.AddGarbage(new GarbageInfo(evil - 2, GarbageType.Evil)); // -2 to specify number of garbage lines
                }

                if (chain.numBlocks > 3)
                {
                    // A combo.
                    // -1 to specify number of garbage blocks instead of combo size
                    chain.AddGarbage(new GarbageInfo(chain.numBlocks - 1, GarbageType.Combo));
                }

                ChainStep(this, chain);

                chain.EndFrame();
            }
            newChain = null;
        }

        public void Update()
        {
            // Send completed chains, if there's something to send.
            foreach (Chain chain in chains)
            {
                if (!chain.IsActive() && (chain.sentCombo || chain.length > 1))
                {
                    ChainFinish(this, chain);
                }
            }

            chains.RemoveAll(chain => !chain.IsActive());
        }

        public delegate void ChainHandler(Popper sender, Chain chain);
        public event ChainHandler ChainStep, ChainFinish;
    }
}
