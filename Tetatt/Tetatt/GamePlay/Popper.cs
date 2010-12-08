using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Tetatt.GamePlay
{
    public class Popper
    {
        List<Chain> chains;
        PlayField pf;
        Chain newChain;

        const int bonusStopTime = 80;

        public Popper(PlayField pf)
        {
            chains = new List<Chain>();
            this.pf = pf;
            newChain = null;
        }

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

        public void Pop()
        {
            bool bBonusStop = false;
            int evil = 0;

            foreach (Chain chain in chains)
            {
                if (!chain.usedThisFrame)
                    continue;

                evil += chain.PopAllAndCountEvil();

                // Check add evil garbage
                if (evil > 0)
                {
                    chain.AddGarbage(new GarbageInfo(evil - 2, GarbageType.Evil)); // -2 to specify number of garbage lines
                }

                bool offscreen = chain.TopMostBlockIndex / PlayField.width < (PlayField.firstVisibleRow - 3); // If the block is too much offscreen

                if (chain.numBlocks > 3)
                {
                    // A combo.
                    // -1 to specify number of garbage blocks instead of combo size
                    chain.AddGarbage(new GarbageInfo(chain.numBlocks - 1, GarbageType.Combo));
                    pf.DelayScroll(chain.numBlocks * 5);
                    if (pf.GetHeight() > PlayField.stressHeight)
                        bBonusStop = true;
                    pf.AddScore((chain.numBlocks - 1) * 10);

                    if (!offscreen) // Only add effect if it's onscreen
                    {
                        pf.ActivatePerformedCombo(
                            chain.TopMostBlockIndex,
                            false,
                            chain.numBlocks);
                    }

                    if (chain.length > 1)
                    {
                        // A chain involving the combo.
                        pf.DelayScroll(chain.numBlocks * 10);
                        pf.AddScore(50 + (chain.length - 1) * 20 * chain.length);

                        if (!offscreen)
                        {
                            pf.ActivatePerformedCombo(
                                chain.TopMostBlockIndex - PlayField.width,
                                true,
                                chain.length);
                        }
                   }
                }
                else if (chain.length > 1)
                {
                    // Just a chain, without a combo
                    pf.DelayScroll(chain.numBlocks * 10);
                    pf.AddScore(50 + (chain.length - 1) * 20 * chain.length);

                    if (!offscreen)
                    {
                        pf.ActivatePerformedCombo(
                            chain.TopMostBlockIndex,
                            true,
                            chain.length);
                    }
                }

                if (bBonusStop)
                {
                    // TODO: Add some nifty graphics, and perhaps not a static bonus?
                    pf.DelayScroll(bonusStopTime);
                }
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
                    foreach (GarbageInfo info in chain.garbage)
                    {
                        pf.AddGarbage(info.size, info.type);
                    }
                    if (chain.length > 1)
                    {
                        pf.AddGarbage(chain.length - 1, GarbageType.Chain);
                    }
                    pf.ActivatePerformedChain(chain);
                }
            }

            chains.RemoveAll(chain => !chain.IsActive());
        }
    }
}
