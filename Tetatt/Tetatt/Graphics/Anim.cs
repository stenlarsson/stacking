using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt.Graphics
{
    public class Anim
    {
        private LinkedList<AnimFrame> frames;
        private LinkedListNode<AnimFrame> curFrame;
        private int animTick;
        private bool reverse;
        private AnimType type;

        public Anim(AnimType type, IEnumerable<AnimFrame> frames)
        {
            this.frames = new LinkedList<AnimFrame>(frames);
            curFrame = this.frames.First;
            animTick = 0;
            reverse = false;
            this.type = type;
        }

        public Anim(int tile)
		    : this(AnimType.Static, new AnimFrame[] {new AnimFrame(tile)})
	    {
        }

        public void Update()
        {
            if (type == AnimType.Static)
                return;

            if (++animTick >= curFrame.Value.delay)
            {
                animTick = 0;
                curFrame = reverse ? curFrame.Previous : curFrame.Next;
                if (type == AnimType.Cycling)
                {
                    if (curFrame == frames.First || curFrame == frames.Last)
                        reverse = !reverse;
                }
                else if (type == AnimType.Looping)
                {
                    if (curFrame == null)
                        curFrame = frames.First;
                }
                else if (type == AnimType.Once)
                {
                    if (curFrame == null)
                    {
                        type = AnimType.Static;
                        curFrame = frames.Last;
                    }
                }
            }
        }

        public bool IsDone()
        {
            return type == AnimType.Static;
        }

        public int GetFrame()
        {
            return curFrame.Value.tile;
        }
    }
}
