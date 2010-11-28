using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.Graphics;

namespace Tetatt.GamePlay
{
    class Block
    {
        protected Anim anim;
        protected BlockType type;
        protected BlockState state;
        protected int popOffset;
        protected int dieOffset;
        protected int dropTimer;
        protected int stateDelay;
        protected BlockState nextState;
        private bool needPopCheck;
        private Chain chain;
        private bool popped;
        protected StressState stress;
        private AnimFrame[] stressFrames;
        private AnimFrame[] flashFrames;

        protected const int dropDelay =  0;
        protected const int flashDelay = 40;

        private const int tileBlank = 63;
        private const int tileNormalOffset = 0;
        private const int tileBounce1Offset = 1;
        private const int tileBounce2Offset = 2;
        private const int tileBounce3Offset = 3;
        private const int tileFlashOffset = 4;
        private const int tileEyesOffset = 5;

        public Block(BlockType type, BlockState state = BlockState.Idle, Chain chain = null, bool needPopCheck = true, Anim anim = null)
        {
            this.anim = anim != null ? anim : new Anim((int)type);
            this.type = type;
            this.state = state;
            popOffset = 0;
            dieOffset = 0;
            dropTimer = 0;
            stateDelay = -1;
            nextState = BlockState.Idle;
            this.needPopCheck = needPopCheck;
            this.Chain = chain;
            popped = false;
            stress = StressState.Normal;

            stressFrames = new AnimFrame[] {
				new AnimFrame((int)type + tileBounce3Offset, 5),
				new AnimFrame((int)type + tileBounce2Offset, 5),
				new AnimFrame((int)type + tileBounce1Offset, 5),
				new AnimFrame((int)type, 5),
			};
            flashFrames = new AnimFrame[] {
				new AnimFrame((int)type),
				new AnimFrame((int)type + tileFlashOffset, 4),
			};

        }

        public virtual void Update()
        {
            if (dropTimer > 0)
                dropTimer--;

            if (stateDelay > 0)
                stateDelay--;

            if (stateDelay == 0)
            {
                stateDelay = -1;
                State = nextState;
            }
            anim.Update();
            popped = false;
        }

        public virtual void Drop()
        {
	        State = BlockState.Falling;
        }

        public virtual void Land()
        {
	        State = BlockState.Idle;
        }

        public virtual void Hover(int delay)
        {
	        stateDelay = delay;
            State = BlockState.Hover;
        }

        public void DropAndLand()
        {
            Drop();
            Land();
        }

	    public bool NeedPopCheck()
        {
            return needPopCheck;
        }

	    public void PopChecked()
        {
            needPopCheck = false;
        }

	    public void ForcePopCheck()
        {
            needPopCheck = true;
        }

	    public void SetPop()
        {
            popped = true;
        }

	    public bool IsPopped()
        {
            return popped;
        }

        public bool CheckDrop()
        {
            // TODO dropDelay is zero, what's the point?
            bool dropTime = (dropTimer <= 0);
            if (dropTime)
                dropTimer = dropDelay;
            return dropTime;
        }

        public bool SameType(Block other)
        {
            return other.Type == type;
        }

        public virtual void Pop(int num, int total)
        {
	        State = BlockState.Flash;
            // TODO difficulty
	        popOffset = 10 + 7 * num;
	        dieOffset = 10 + 7 * total - popOffset - 1;
        }

        public virtual void Move()
        {
	        State = BlockState.Moving;
        }

        public int Tile
        {
            get { return anim.GetFrame(); }
        }

        public BlockType Type
        {
            get { return type; }
        }

        public BlockState State
        {
            get { return state; }
            set { ChangeState(value); }
        }

        public Chain Chain
        {
            get { return chain; }
            set
            {
                if (chain != null)
                    chain.activeBlocks--;
                chain = value;
                if (chain != null)
                    chain.activeBlocks++;
            }
        }
 
        public StressState Stress
        {
            set
            {
	            if(state != BlockState.Idle || stress == value)
		            return;

                switch(value)
	            {
	            case StressState.Normal:
		            anim = new Anim((int)type);
		            break;
	            case StressState.Stop:
		            anim = new Anim((int)type + tileBounce3Offset);
		            break;
	            case StressState.Stress:
			        anim = new Anim(AnimType.Looping, stressFrames);
		            break;
	            }
	            stress = value;
            }
        }

        protected virtual void ChangeState(BlockState newState)
        {
            state = newState;
	        switch(newState)
	        {
	        case BlockState.Idle:
			    if(stress == StressState.Stress)
				    anim = new Anim(AnimType.Looping, stressFrames);
			    else if(state == BlockState.Falling)
				    anim = new Anim(AnimType.Once, stressFrames);
			    stateDelay = -1;
			    ForcePopCheck();
		        break;
	        case BlockState.Falling:
		        dropTimer = dropDelay;
		        stateDelay = -1;
		        break;
	        case BlockState.Hover:
		        nextState = BlockState.Falling;
		        ForcePopCheck(); // needed for lateslip-technique
		        break;
	        case BlockState.Moving:
		        anim = new Anim((int)type);
		        nextState = BlockState.PostMove;
		        stateDelay = 5;
		        break;
	        case BlockState.PostMove:
		        anim = new Anim((int)type);
		        nextState = BlockState.Idle;
		        stateDelay = 1;
		        break;
	        case BlockState.Flash:
		        {
			        anim = new Anim(AnimType.Looping, flashFrames);
			        nextState = BlockState.Pop;
                    // TODO difficulty
                    stateDelay = 40;
			        //stateDelay = g_game->GetLevelData()->flashTime;
		        }
		        break;
	        case BlockState.Pop:
		        anim = new Anim((int)type + tileEyesOffset);
		        nextState = BlockState.Pop2;
		        stateDelay = popOffset;
		        break;
            case BlockState.Pop2:
                nextState = BlockState.Pop3;
		        stateDelay = 1;
		        break;
            case BlockState.Pop3:
		        anim = new Anim(tileBlank);
		        nextState = BlockState.Dead;
		        stateDelay = dieOffset;
		        break;
	        case BlockState.Dead:
		        stateDelay = -1;
		        break;
	        }
        }

        public virtual Block ReplaceBlock()
        {
            Chain = null;
            return null;
        }
    }
}
