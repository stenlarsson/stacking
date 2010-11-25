using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.GamePlay
{
    class Popper
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
		        if(!chain.usedThisFrame)
			        continue;

		        evil += chain.PopAllAndCountEvil();

		        // Check add evil garbage
		        if(evil > 0)
		        {
			        chain.AddGarbage(new GarbageInfo(evil-2, GarbageType.Evil)); // -2 to specify number of garbage lines
		        }
		
		        bool offscreen = chain.TopMostBlockIndex/PlayField.width < (PlayField.firstVisibleRow-3); // If the block is too much offscreen
		
		        if(chain.numBlocks > 3)
		        {
			        // A combo.
			        // -1 to specify number of garbage blocks instead of combo size
			        chain.AddGarbage(new GarbageInfo(chain.numBlocks-1, GarbageType.Combo));
			        pf.DelayScroll(chain.numBlocks*5);
			        if(pf.GetHeight() > PlayField.stressHeight)
				        bBonusStop = true;
			        pf.AddScore((chain.numBlocks-1)*10);
			
                    // TODO effect
                    /*
			        if(!offscreen) // Only add effect if it's onscreen
				        eh->Add(
					        new EffCombo(
						        (*it)->TopMostBlockIndex,
						        COMBO_4,
						        (*it)->numBlocks));
                    */
			        if(chain.length > 1)
			        {
				        // A chain involving the combo.
				        pf.DelayScroll(chain.numBlocks*10);
				        pf.AddScore(50+(chain.length-1)*20*chain.length);

                        // TODO effect
                        /*
				        if(!offscreen)
					        eh->Add(
						        new EffCombo(
							        (*it)->TopMostBlockIndex-PF_WIDTH,
							        COMBO_2X,
							        (*it)->length));
                        */
			        }
                    // TODO sound
                    /*
			        Sound::PlayChainStepEffect(*it);
                    */
		        }
		        else if(chain.length > 1)
		        {
			        // Just a chain, without a combo
			        pf.DelayScroll(chain.numBlocks*10);
			        pf.AddScore(50+(chain.length-1)*20*chain.length);

                    // TODO effect
                    /*
			        if(!offscreen)
				        eh->Add(
					        new EffCombo(
						        (*it)->TopMostBlockIndex,
						        COMBO_2X,
						        (*it)->length));
			        if(pf->GetHeight() > PF_STRESS_HEIGHT)
				        bBonusStop = true;
                    */
                    // TODO sound
                    /*
                    Sound::PlayChainStepEffect(*it);
                    */
		        }

		        if(bBonusStop)
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
            chains.RemoveAll(chain => !chain.IsActive());

            // TODO send chains
            /*
            std::vector<Chain*>::iterator keep =
        		std::partition(chains.begin(), chains.end(), std::mem_fun(&Chain::IsActive));
	
	        // Send completed chains, if there's something to send.
	        for(std::vector<Chain*>::iterator it = keep; it != chains.end(); ++it)
	        {
		        if((*it)->bSentCombo || (*it)->length > 1)
		        {
			        for(std::vector<GarbageInfo>::iterator j = (*it)->garbage.begin(); j != (*it)->garbage.end(); j++)
				        g_game->SendGarbage(j->size, j->type);
			        if((*it)->length > 1)
				        g_game->SendGarbage((*it)->length-1, GARBAGE_CHAIN);
			        Sound::PlayChainEndEffect(*it);
		        }
	        }

	        delete_each(keep, chains.end());
	        chains.erase(keep, chains.end());
            */
        }
    }
}
