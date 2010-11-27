using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Tetatt.Graphics;

namespace Tetatt.GamePlay
{
    class GarbageBlock : Block
    {
        private BlockType typeAfterPop;
        private BigGarbageBlock gb;
	    private int nextGraphic;
	    private bool wantToDrop;

        private AnimFrame[] flashFrames;

        public GarbageBlock(GarbageType garbageType, BlockType typeAfterPop, BigGarbageBlock gb)
            : base(garbageType == GarbageType.Evil ? BlockType.EvilGarbage : BlockType.Garbage,
            BlockState.Idle,
            null,
            false,
            new Anim(BigGarbageBlock.graphicsSingle))
        {
            this.typeAfterPop = typeAfterPop;
            this.gb = gb;
            nextGraphic = BigGarbageBlock.graphicsDisabled;
            wantToDrop = false;

            flashFrames = new AnimFrame[] {
                new AnimFrame((int)type + BigGarbageBlock.tileFlash1Offset, 1),
                new AnimFrame((int)type + BigGarbageBlock.tileFlash2Offset, 4)
            };
        }

        public void SetGraphic(int newGraphic)
        {
	        anim = new Anim(newGraphic);
        }

        public override void Drop()
        {
	        if(!wantToDrop)
	        {
		        gb.Drop();
		        wantToDrop = true;
	        }
        }

        public override void Land()
        {
	        gb.Land();
        }

        public override void Hover(int delay)
        {
	        gb.Hover(delay);
        }

        public BigGarbageBlock GB
        {
            get { return gb; }
        }

 	    public bool IsOtherGarbageType(Block block)
        {
		    return block.Type ==
			    (Type == BlockType.Garbage ? BlockType.EvilGarbage : BlockType.Garbage);
	    }

        public void Pop(int num, int total, int nextGraphic)
        {
            base.Pop(num, total);
            this.nextGraphic = nextGraphic;
        }

        protected override void ChangeState(BlockState newState)
        {
            state = newState;
            switch (newState)
            {
                case BlockState.Idle:
                    stateDelay = -1;
                    break;
                case BlockState.Falling:
                    dropTimer = dropDelay;
                    stateDelay = -1;
                    break;
                case BlockState.Hover:
                    nextState = BlockState.Falling;
                    break;
                case BlockState.Flash:
                    anim = new Anim(AnimType.Looping, flashFrames);
                    nextState = BlockState.Pop;
                    // TODO difficulty
                    stateDelay = 40;
                    //stateDelay = g_game->GetLevelData()->flashTime;
                    break;
                case BlockState.Pop:
                    anim = new Anim((int)type + BigGarbageBlock.tileFlash1Offset);
                    nextState = BlockState.Pop2;
                    stateDelay = popOffset;
                    break;
                case BlockState.Pop2:
                    nextState = BlockState.Pop3;
                    stateDelay = 1;
                    break;
                case BlockState.Pop3:
                    anim = new Anim(nextGraphic == BigGarbageBlock.graphicsDisabled ? (int)typeAfterPop : nextGraphic);
                    nextState = BlockState.Dead;
                    stateDelay = dieOffset;
                    break;
                case BlockState.Dead:
                    if (nextGraphic != BigGarbageBlock.graphicsDisabled)
                    {
                        nextGraphic = BigGarbageBlock.graphicsDisabled;
                        state = BlockState.Falling;
                        dropTimer = dropDelay;
                    }
                    stateDelay = -1;
                    break;
                default:
                    Debug.Fail(string.Format("Unexpected state {0}", state));
                    break;
            }
        }
        
        public override void Update()
        {
	        base.Update();
	        wantToDrop = false;
        }

        public Block CreateBlock()
        {
	        return new Block(typeAfterPop, BlockState.Falling, this.Chain, false);
        }

        public override Block ReplaceBlock() {
            // Replace garbage with a real block
            Block b = CreateBlock();
            gb.RemoveBlock();
            base.ReplaceBlock();
            return b;
        }
    }
}
