using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Tetatt.Graphics;

namespace Tetatt.GamePlay
{
    public class PlayField
    {
        public const int blockSize = 32;
        public const int width = 6;
        public const int height = 48;
        public const int startHeight = 8;
        public const int visibleHeight = 12;
        public const int stressHeight = 10;
        public const int firstVisibleRow = (height - visibleHeight - 1);
        public readonly static Pos markerStart = new Pos(visibleHeight/2 - 1, width/2 - 1);
        public const double grayBlockChance = 0.10; // chance per block on a row of six
        public const int grayBlockDelay = 5; // number of rows to wait before they can appear
        public const int deathDelay = 50; // num frames before dying
        public const int deathDuration = 52; // num frames in Die state
        public const int maxStopTime = 180;

        private Block[,] field;
        private int[] fieldHeight;
        private Pos markerPos;
        private Popper popper;
        private int makerHeightLimit {
            get {
                return (GetHeight() >= visibleHeight ? visibleHeight : visibleHeight-1);
            }
        }



        private bool fastScroll;
        private double scrollOffset;
        private int scrollPause;

        //private EffectHandler effects;

        private int swapTimer;

        private PlayFieldState state;
        private int stateDelay;

        private bool tooHigh;
        private int dieTimer;

        private bool musicNormal;
        private bool musicDanger;
        private int normalMusicDelay;

        private int score;
        private int timeTicks;
        private int scrolledRows;

        private GarbageHandler gh;

        private bool leftAlignGarbage;

        // TODO accessor
        public static TileSet blocksTileSet;
        public static Texture2D background;
        public static Texture2D marker;

        public struct Pos
        {
            public int Row, Col;
            public Pos(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }

        public PlayField()
        {
            field = new Block[height,width];
            fieldHeight = new int[width];
            popper = new Popper(this);
            markerPos = markerStart;

            fastScroll = false;
            scrollPause = 0;
            scrollOffset = 0;

            // TODO
            //this.effects = effects;

            swapTimer = 0;

            state = PlayFieldState.Init;
            stateDelay = -1;

            tooHigh = false;
            dieTimer = 0;

            musicNormal = false;
            musicDanger = false;
            normalMusicDelay = 0;

            score = 0;
            timeTicks = 0;
            scrolledRows = 0;

            gh = new GarbageHandler();

            leftAlignGarbage = false;

            RandomizeField();
        }

        public void DelayScroll(int delay)
        {
            scrollPause = Math.Min(maxStopTime, scrollPause + delay);
        }

        public void AddScore(int score)
        {
            this.score += score;
        }

        private void RandomizeField()
        {
            for (int i = startHeight-1; i >= 0; i--)
                RandomizeRow(i);
        }

        // NOTE: Assumes that rows are generated top to bottom
        private void RandomizeRow(int row)
        {
            bool grayBlock = false;
            for (int col = 0; col < width; col++)
            {
                field[row,col] = null;
                while (field[row,col] == null)
                {
                    BlockType type = RandomBlocks.Next(
                        (!grayBlock && scrolledRows > grayBlockDelay) ? grayBlockChance : 0.0);
                    // make sure block won't pop immediately

                    if (col >= 2 &&
                        IsOfType(field[row,col-1], type) &&
                        IsOfType(field[row,col-2], type))
                        continue;

                    if (IsOfType(field[row+1,col], type) &&
                        IsOfType(field[row+2,col], type))
                        continue;

                    grayBlock = grayBlock || (type == BlockType.Gray);
                    field[row,col] = new Block(type);
                }
            }
        }

        public void Start()
        {
            state = PlayFieldState.Start;
            stateDelay = 150;
            // TODO effects
            Debug.WriteLine("Start");
            //effects->Add(new EffReady());
            // TODO music
            //Sound::PlayMusic(false);
            musicNormal = true;
        }

        public void Update()
        {
            StateCheck();

            if (state == PlayFieldState.Play)
            {
                if (swapTimer > 0)
                {
                    if (SwapBlocks())
                        swapTimer = 0;
                    else
                        swapTimer--;
                }
                foreach (Block b in field)
                {
                    if (b != null)
                        b.Update();
                }
                gh.DropGarbage(this);
                gh.Update();
                popper.Update();
                if (ShouldScroll())
                    ScrollField();
                ClearDeadBlocks();
                DropBlocks();
                CheckForPops();
                CheckHeight();

                // TODO
                timeTicks++;
            }
            else if (state == PlayFieldState.Die)
            {
                // Shake screen
                if (stateDelay > 0)
                {
                    if ((stateDelay & 1) == 1)
                    {
                        if ((stateDelay & 2) == 2)
                            scrollOffset += 5;
                        else
                            scrollOffset -= 5;
                    }
                }
            }

            // TODO
            //effects->Tick();
        }

        public void KeyInput(InputType input)
        {
            switch (input)
            {
                case InputType.Up:
                    swapTimer = 0;
                    if (markerPos.Row < makerHeightLimit)
                        markerPos.Row++;
                    break;

                case InputType.Left:
                    swapTimer = 0;
                    if (markerPos.Col > 0)
                        markerPos.Col--;
                    break;

                case InputType.Right:
                    swapTimer = 0;
                    if (markerPos.Col < width-2)
                        markerPos.Col++;
                    break;

                case InputType.Down:
                    swapTimer = 0;
                    if (markerPos.Row > 1)
                        markerPos.Row--;
                    break;

                case InputType.Swap:
                    if (state != PlayFieldState.Play)
                        break;
                    if (!SwapBlocks())
                        swapTimer = 20;
                    else
                        swapTimer = 0;
                    break;

                case InputType.Raise:
                    fastScroll = true;
                    if (scrollPause > 2)
                        scrollPause = 0;
                    break;
            }
        }

        private void StateCheck()
        {
            if (stateDelay > 0)
            {
                stateDelay--;
                return;
            }

            if (state == PlayFieldState.Start)
            {
                state = PlayFieldState.Play;
            }

            if (state == PlayFieldState.Die)
            {
                state = PlayFieldState.Dead;
                // TODO
                //g_game->PlayerDied();
            }
        }

        private bool SwapBlocks()
        {
            Debug.Assert(markerPos.Row > 0);

            Block left = field[markerPos.Row, markerPos.Col];
            Block right = field[markerPos.Row, markerPos.Col+1];

            if (left != null && (!IsOfState(left, BlockState.Idle) || IsGarbage(left)))
                return false;
            if (right != null && (!IsOfState(right, BlockState.Idle) || IsGarbage(right)))
                return false;

            if (left == null && right == null)
                return false;

            if (left == null)
            {
                if (IsHoverOrMove(field[markerPos.Row+1,markerPos.Col]))
                    return false;

                if (IsOfState(field[markerPos.Row+1,markerPos.Col+1], BlockState.Idle))
                    field[markerPos.Row+1,markerPos.Col+1].Hover(9);
            }
            else if (right == null)
            {
                if (IsHoverOrMove(field[markerPos.Row+1,markerPos.Col+1]))
                    return false;

                if (IsOfState(field[markerPos.Row+1,markerPos.Col], BlockState.Idle))
                    field[markerPos.Row+1,markerPos.Col].Hover(9);
            }

            field[markerPos.Row, markerPos.Col] = right;
            field[markerPos.Row, markerPos.Col+1] = left;

            if (right != null)
            {
                right.Move();
                // TODO effects
                //effects->Add(new EffMoveBlock(DIR_LEFT, right, RightOf(pos)));
            }
            if (left != null)
            {
                left.Move();
                // TODO effects
                //effects->Add(new EffMoveBlock(DIR_RIGHT, left, pos));
            }
            return true;
        }

        private bool ShouldScroll()
        {
            for (int row = 1; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (field[row,col] == null)
                        continue;

                    if (field[row,col].State != BlockState.Idle)
                    {
                        fastScroll = false;
                        return false;
                    }
                }
            }

            if (scrollPause != 0)
            {
                scrollPause--;
                fastScroll = false;
                return false;
            }

            return true;
        }

        void ScrollField()
        {
            if (!tooHigh)
            {
                scrolledRows++;

                if (fastScroll)
                    scrollOffset -= 2;
                else
                    // TODO difficulty
                    scrollOffset -= 0.052;

                if (scrollOffset <= -blockSize)
                {
                    for (int row = height - 1; row >= 1; row--)
                        for (int col = 0; col < width; col++)
                            field[row,col] = field[row-1,col];
                    RandomizeRow(0);

                    if (markerPos.Row < makerHeightLimit)
                    {
                        markerPos.Row++;
                    }

                    if (GetHeight() == visibleHeight)
                    {
                        scrollOffset = 0;
                    }
                    else
                    {
                        scrollOffset += blockSize;
                        if (fastScroll)
                        {
                            score++;
                            scrollPause = 2;
                            fastScroll = false;
                        }
                    }
                }
            }
            else
            {
                if (++dieTimer > deathDelay)
                {
                    state = PlayFieldState.Die;
                    stateDelay = deathDuration;
                    // TODO sound
                    //Sound::PlayDieEffect();
                }
            }
        }

        private void ClearDeadBlocks()
        {
            for (int row = height-2; row >= 1; row--)
            {
                for (int col = 0; col < width; col++)
                {
                    if (field[row,col] == null || field[row,col].State != BlockState.Dead)
                        continue;

                    if(!IsGarbage(field[row,col]))
                    {
                        Chain chain = field[row,col].Chain;
                        // Propagate chain upwards, and drop idle blocks
                        for (int y = row+1; y < height && IsDropable(field[y,col]); y++)
                        {
                            field[y,col].Chain = chain;
                            if (field[y,col].State == BlockState.Idle)
                                field[y,col].Drop();
                        }
                    }
                    field[row,col] = field[row,col].ReplaceBlock();
                }
            }
        }

        private void DropBlocks()
        {
            //loop through field, starting at the bottom
            for (int row = 1; row < height; row++)
            {
                for (int col = width-1; col >= 0; col--)
                {
                    if (field[row,col] == null)
                        continue;


                    if (field[row,col].State == BlockState.PostMove)
                    {
                        if (field[row-1,col] == null)
                        {
                            //hover a bit before dropping  and don't pop
                            field[row,col].Hover(4);
                            field[row,col].PopChecked();
                            continue;
                        }

                        if (IsOfState(field[row+1,col], BlockState.Hover))
                        {
                            //if there's a block above, and it's hovering
                            //(which it will be if it dropped on this block while it was MOVING)

                            field[row,col].Land(); //land this block now instead of next tick

                            for (int y = row+1; y < height && IsOfState(field[y,col], BlockState.Hover); y++)
                                field[y,col].DropAndLand();
                        }
                        continue;
                    }

                    if (row >= 2 && field[row,col].State == BlockState.Idle)
                    {
                        if (field[row-1,col] == null)
                            field[row,col].Drop();
                        else if (field[row-1,col].State == BlockState.Falling)
                        {
                            field[row,col].Drop();
                            if (IsBlock(field[row,col]))
                                field[row,col].Chain = field[row-1,col].Chain;
                        }
                    }

                    if (field[row,col].State == BlockState.Falling)
                    {
                        if (IsHoverOrMove(field[row-1, col]))
                        {
                            field[row,col].Hover(1500);//we hover this block a while
                            //(it won't hover this long though since
                            //it'll drop when the ones below drop
                            // BUG, sometimes it keeps hovering, why??
                            continue;

                        }
                        else if (field[row-1,col] != null && field[row-1,col].State != BlockState.Falling)
                        {
                            field[row,col].Land();
                            continue;
                        }
                        if (row < height-1 && IsHoverOrIdle(field[row+1,col]))
                        {
                            // This happens when a stack of blocks 'landed'
                            // on a block the player moved in below the stack,
                            // and it's time to start falling again (BST_HOVER),
                            // or when a block is pulled out of a row and the
                            // block above where it was has just stopped hovering (BST_IDLE)
                            field[row+1,col].Drop();
                        }
                    }

                    if (IsOfState(field[row,col], BlockState.Hover))
                        if (IsOfState(field[row-1,col], BlockState.Idle) || IsOfState(field[row-1,col], BlockState.Flash))
                            field[row,col].DropAndLand();

                }
            }

            //loop through field, starting at the bottom
            for (int row = 1; row < height-1; row++)
            {
                for (int col = width-1; col >= 0; col--)
                {
                    if (IsOfState(field[row,col], BlockState.Falling))
                    {
                        if (field[row,col].CheckDrop())	//if it's time to really drop the block
                        {
                            Debug.Assert(field[row-1,col] == null);
                            field[row-1,col] = field[row,col];
                            field[row,col] = null;
                        }
                    }
                }
            }
        }

        private void CheckForPops()
        {
            bool bPop = false; //flag to see if any pops occured this tick
            bool bClearChain = true; //flag to clear a blocks chain
            Chain tmpChain = null;

            //here's a good place to update the height
            for (int i = 0; i < fieldHeight.Length; i++)
            {
                fieldHeight[i] = -1;
            }

            for (int row = height-2; row >= 1; row--)
            {
                for (int col = 0; col < width; col++)
                {
                    bClearChain = true;
                    if (field[row,col] == null)//skip if there's no block
                        continue;

                    if (field[row,col].State != BlockState.Falling)// no falling blocks please
                        if (fieldHeight[col] == -1)//if we haven't already found one in the same column
                            fieldHeight[col] = row+1;//set height

                    if (IsGarbage(field[row,col]) || !field[row,col].NeedPopCheck())
                    {
                        if (field[row,col].State == BlockState.Pop2)//but we want to check if we need to 'pop' it
                        {
                            if (!IsGarbage(field[row,col]))
                            {
                                score += 10;
                            }

                            if (row <= visibleHeight)
                                Popped(this, new PoppedEventArgs(
                                    PosToVector(new Pos(row, col)),
                                    IsGarbage(field[row,col]),
                                    field[row,col].Chain));
                        }
                        continue;
                    }

                    int top = row;
                    for (; top < height-1; top++)
                    {
                        if (!IsPopableWith(field[row,col], field[top+1,col]))
                            break;
                    }

                    tmpChain = null;

                    int bottom = top;
                    for (; bottom >= 1; bottom--)
                    {
                        // TODO: Move the chain thing after we know the block is used
                        if (tmpChain == null)
                            tmpChain = field[bottom,col].Chain; //if one of the blocks is in a chain
                        //this'll store that
                        if (!IsPopableWith(field[row,col], field[bottom-1,col]))
                            break;
                    }

                    if (top - bottom >= 2) // Distance between 3 adjacent blocks is 2
                    {
                        for (int check = bottom; check <= top; check++)
                        {
                            if (!field[check,col].IsPopped())
                            {
                                field[check,col].Chain = tmpChain;
                                popper.AddBlock(field[check,col], (height - check)*width+col);
                                field[check,col].SetPop();
                            }
                        }
                        bClearChain = false;//and make sure we don't clear the chain
                        bPop = true;//and set popflag
                    }

                    int left = col;
                    for (; left > 0; left--)
                    {
                        if (!IsPopableWith(field[row,col], field[row,left-1]))
                            break;
                    }

                    tmpChain = null;

                    int right = left;
                    for (; right < width-1; right++)
                    {
                        // TODO: Move until we know block should be in chain
                        if (tmpChain == null)
                            tmpChain = field[row,right].Chain;//store the chain

                        if (!IsPopableWith(field[row,col], field[row,right+1]))
                            break;
                    }

                    if (right - left >= 2) // Distance between 3 adjacent blocks is 2
                    {
                        for (int check = left; check <= right; check++)
                        {
                            if (!field[row,check].IsPopped())
                            {
                                field[row,check].Chain = tmpChain;
                                popper.AddBlock(field[row,check], row*width + check);
                                field[row,check].SetPop();
                            }
                        }
                        bClearChain = false;
                        bPop = true;
                    }

                    field[row,col].PopChecked();
                    if (bClearChain && field[row,col].State != BlockState.Hover)
                        field[row,col].Chain = null;
                }
            }

            // TODO: Should tmpChain not be reset after the loop above?

            if (bPop)//if something popped
            {
                popper.Pop();//initiate popping

                bool bDoOver = true;
                bool bReverse = false;
                while (bDoOver) // Check again to catch garbage popping garbage below itself
                {
                    bDoOver = false;
                    for (int row = 0; row < height-1; row++)//loop, bottom to top
                    {
                        for (int col = width-1; col >= 0; col--)
                        {
                            // This checks if there are GarbageBlocks that needs to pop
                            // TODO: Do this in terms of the larger GarbageBlocks instead of the individual fields
                            if (!IsGarbage(field[row,col]) || field[row,col].State != BlockState.Idle)
                                continue;
                            if (field[row,col].IsPopped())
                                continue;

                            GarbageBlock g = (GarbageBlock)field[row,col];
                            tmpChain = SamePopChain(g, field[row-1,col], tmpChain);
                            tmpChain = SamePopChain(g, field[row+1,col], tmpChain);
                            if (col < width-1)
                                tmpChain = SamePopChain(g, field[row,col+1], tmpChain);
                            if (col > 0)
                                tmpChain = SamePopChain(g, field[row,col-1], tmpChain);

                            if (!g.IsPopped()) // If it didn't pop, next!
                                continue;

                            if (tmpChain != null)
                            {
                                gh.AddPop(g.GB, tmpChain, bReverse);
                                bDoOver = true;
                                tmpChain = null;
                            }
                        }
                    }
                    if (bDoOver && !bReverse)
                        bReverse = true;
                }

                gh.Pop();
            }
        }

        private void CheckHeight()
        {
            tooHigh = (GetHeight() > visibleHeight);

            if (!tooHigh)
                dieTimer = 0;

            for (int i = 0; i < width; i++)
            {
                StressState stress =
                    (fieldHeight[i] >= stressHeight) ?
                        ((tooHigh || scrollPause > 0) ? StressState.Stop : StressState.Stress) :
                        StressState.Normal;
                for (int row = 1; row < height; row++)
                {
                    if (IsBlock(field[row,i]))
                        field[row,i].Stress = stress;
                }
            }

            // TODO music
            /*
	        if(GetHeight() >= PF_STRESS_HEIGHT)
	        {
		        if(!bMusicDanger)
		        {
			        Sound::PlayMusic(true);
			        bMusicDanger = true;
			        bMusicNormal = false;
		        }
		        normalMusicDelay = 20;
	        }
	        else
	        {
		        if(!bMusicNormal && --normalMusicDelay <= 0)
		        {
			        Sound::PlayMusic(false);
			        bMusicNormal = true;
			        bMusicDanger = false;
		        }
	        }
            */
        }

        private bool IsLineOfFieldEmpty(int row)
        {
            for (int i = 0; i < width; i++)
            {
                if (field[row,i] != null)
                    return false;
            }
            return true;
        }

        public bool InsertGarbage(BigGarbageBlock b)
        {
            int row = Math.Min(GetHeight() + 1, visibleHeight);

            // Get the position where we should start inserting
            // Unfortunately GetHeight only counts normal blocks..
            while (row < height && !IsLineOfFieldEmpty(row))
                row++;

            int lines = (b.GetNum()+width-1)/width;
            if (row + lines >= height)
            {
                // Out of space to drop blocks
                return false;
            }

            int col = 0;
            if (leftAlignGarbage && lines == 1)
                col = width - b.GetNum();
            for (int i = b.GetNum() - 1; i >= 0; i--)
            {
                Debug.Assert(field[row,col + i % width] == null);
                field[row,col + i % width] = b.GetBlock(i);
                if (col + i % width == 0)
                    row++;
            }

            leftAlignGarbage = !leftAlignGarbage;

            return true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            // Draw frame and background
            spriteBatch.Begin();
            spriteBatch.Draw(
                background,
                offset - new Vector2(16, 16), // Adjust for the frame
                Color.White);
            spriteBatch.End();

            // Setup sprite clipping using scissor test
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null,
                new RasterizerState()
                {
                    ScissorTestEnable = true
                });
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                (int)offset.X,
                (int)offset.Y,
                width * blockSize,
                visibleHeight * blockSize);

            offset.Y += (int)scrollOffset;

            // Draw blocks
            for (int row = 0; row < visibleHeight+1; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Block block = field[row,col];
                    if (block != null)
                    {
                        int tile = block.Tile;
                        Vector2 pos = PosToVector(new Pos(row, col)) + offset;
                        spriteBatch.Draw(
                            blocksTileSet.Texture,
                            new Rectangle(
                                (int)pos.X,
                                (int)pos.Y,
                                blockSize,
                                blockSize),
                            blocksTileSet.SourceRectangle(tile),
                            (row == 0) ? Color.DarkGray : Color.White);
                    }
                }
            }

            spriteBatch.End();

            // Draw frame and background
            spriteBatch.Begin();
            spriteBatch.Draw(
                marker,
                PosToVector(markerPos) + offset - new Vector2(4, 5),
                Color.White);
            spriteBatch.End();
        }

        private bool IsOfType(Block b, BlockType t)
        {
            return b != null && b.Type == t;
        }
        private bool IsGarbage(Block b)
        {
            return b != null && (b.Type == BlockType.Garbage || b.Type == BlockType.EvilGarbage);
        }
        private bool IsBlock(Block b)
        {
            return b != null && b.Type != BlockType.Garbage && b.Type != BlockType.EvilGarbage;
        }
        private bool IsOfState(Block b, BlockState s)
        {
            return b != null && b.State == s;
        }
        public int GetHeight()
        {
            return fieldHeight.Max();
        }
        private bool IsHoverOrMove(Block b)
        {
            if (b == null)
                return false;
            switch (b.State)
            {
                case BlockState.Hover:
                case BlockState.Moving:
                case BlockState.PostMove:
                    return true;
                default:
                    return false;
            }
        }
        private bool IsHoverOrIdle(Block b)
        {
            if (b == null)
                return false;
            switch (b.State)
            {
                case BlockState.Hover:
                case BlockState.Idle:
                    return true;
                default:
                    return false;
            }
        }
        private bool IsPopableWith(Block reference, Block candidate)
        {
            return IsHoverOrIdle(candidate) && candidate.SameType(reference);
        }

        Chain SamePopChain(GarbageBlock g, Block block, Chain chain)
        {
            if (block != null &&
                block.IsPopped() &&
                !g.IsOtherGarbageType(block))
            {
                if (chain == null)
                    chain = block.Chain;
                g.GB.InitPop(chain);
            }
            return chain;
        }

        bool IsDropable(Block b)
        {
            return IsBlock(b) && (b.State == BlockState.Idle || IsHoverOrMove(b));
        }

        public void AddGarbage(int num, GarbageType type)
        {
            gh.AddGarbage(num, type);
        }

        public Vector2 PosToVector(Pos pos)
        {
            return new Vector2(
                pos.Col * blockSize,
                (visibleHeight - pos.Row) * blockSize);
        }

        public void ActivatePerformedCombo(int pos, bool isChain, int count)
        {
            ComboEventArgs eventArgs = new ComboEventArgs(
                PosToVector(new Pos(height - pos / width, pos % width)),
                isChain,
                count);
            PerformedCombo(this, eventArgs);
        }
        public event EventHandler<ComboEventArgs> PerformedCombo;

        public void ActivatePerformedChain(Chain chain)
        {
            ChainEventArgs eventArgs = new ChainEventArgs(
                chain);
            PerformedChain(this, eventArgs);
        }
        public event EventHandler<ChainEventArgs> PerformedChain;

        public event EventHandler<PoppedEventArgs> Popped;
    }
}
