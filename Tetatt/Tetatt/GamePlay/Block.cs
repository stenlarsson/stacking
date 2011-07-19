using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.Graphics;

namespace Tetatt.GamePlay
{
    public class Block
    {
        protected Anim anim;
        protected BlockType type;
        protected BlockState state;
        protected int popOffset;
        protected int dieOffset;
        protected int dropTimer;
        public int StateDelay;
        protected BlockState nextState;
        public bool NeedPopCheck;
        private Chain chain;
        public bool Popped;
        protected StressState stress;
        private AnimFrame[] stressFrames;
        private AnimFrame[] flashFrames;
        public bool Right;

        protected const int dropDelay =  0;
        protected const int flashDelay = 40;

        private const int tileBlank = 127;
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
            StateDelay = -1;
            nextState = BlockState.Idle;
            this.NeedPopCheck = needPopCheck;
            this.Chain = chain;
            this.Popped = false;
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

            if (StateDelay > 0)
                StateDelay--;

            if (StateDelay == 0)
            {
                StateDelay = -1;
                State = nextState;
            }
            anim.Update();
            Popped = false;
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
	        StateDelay = delay;
            State = BlockState.Hover;
        }

        public void DropAndLand()
        {
            Drop();
            Land();
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

        public virtual void Pop(int num, int total, int popStartOffset, int popTime, int flashTime)
        {
	        State = BlockState.Flash;
            StateDelay = flashTime;
	        popOffset = popStartOffset + popTime * num;
	        dieOffset = popStartOffset + popTime * total - popOffset - 1;
        }

        public virtual void Move(bool right)
        {
	        State = BlockState.Moving;
            Right = right;
        }

        public int Tile
        {
            get { return anim.GetFrame(); }
        }

        public BlockType Type
        {
            get { return type; }
        }

        public bool IsState(params BlockState[] states)
        {
            for (int i = 0; i < states.Length; i++) {
                if (states[i] == state)
                    return true;
            }
            return false;
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
			    StateDelay = -1;
			    NeedPopCheck = true;
		        break;
	        case BlockState.Falling:
		        dropTimer = dropDelay;
		        StateDelay = -1;
		        break;
	        case BlockState.Hover:
		        nextState = BlockState.Falling;
		        NeedPopCheck = true; // needed for lateslip-technique
		        break;
	        case BlockState.Moving:
		        anim = new Anim((int)type);
		        nextState = BlockState.PostMove;
		        StateDelay = 5;
		        break;
	        case BlockState.PostMove:
		        anim = new Anim((int)type);
		        nextState = BlockState.Idle;
		        StateDelay = 1;
		        break;
	        case BlockState.Flash:
		        anim = new Anim(AnimType.Looping, flashFrames);
		        nextState = BlockState.Pop;
		        break;
	        case BlockState.Pop:
		        anim = new Anim((int)type + tileEyesOffset);
		        nextState = BlockState.Pop2;
		        StateDelay = popOffset;
		        break;
            case BlockState.Pop2:
                nextState = BlockState.Pop3;
		        StateDelay = 1;
		        break;
            case BlockState.Pop3:
		        anim = new Anim(tileBlank);
		        nextState = BlockState.Dead;
		        StateDelay = dieOffset;
		        break;
	        case BlockState.Dead:
		        StateDelay = -1;
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
