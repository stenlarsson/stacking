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
    class PlayField
    {
        public const int blockSize = 32;
        public const int width = 6;
        public const int height = 48;
        public const int startHeight = 8;
        public const int visibleHeight = 12;
        public const int stressHeight = 10;
        public const int firstVisibleRow = (height - visibleHeight - 1);
        public const int firstFilledRow = height - startHeight;
        public const int firstBlockFirstVisibleRow = firstVisibleRow * width;
        public const int firstBlockSecondVisibleRow = (firstVisibleRow + 1) * width;
        public const int firstBlockLastRow = (height - 1) * width;
        public const int lastBlockSecondLastRow = firstBlockLastRow - 1;
        public const int firstBlockSecondLastRow = (height - 2) * width;
        public const int numBlocks = width * height;
        public const int markerStart = numBlocks - width * 6 - (width / 2) - 1;
        public const int garbageDropStart = firstBlockFirstVisibleRow - 2 * width - 1;
        public const double grayBlockChance = 0.10; // chance per block on a row of six
        public const int grayBlockDelay = 5; // number of rows to wait before they can appear
        public const int deathDelay = 50; // num frames before dying
        public const int deathDuration = 52; // num frames in Die state
        public const int maxStopTime = 180;

        private Block[] field;
        private int[] fieldHeight;
        private int markerPos;
        private Popper popper;

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

        private static Random random = new Random();

        // TODO accessor
        public static TileSet blocksTileSet;
        public static Texture2D background;
        public static Texture2D marker;

        public PlayField()
        {
            field = new Block[numBlocks];
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
            for (int i = firstFilledRow; i < height; i++)
                RandomizeRow(i);
        }

        private void RandomizeRow(int row)
        {
            bool grayBlock = false;
            for (int i = row * width; i < (row + 1) * width; i++)
            {
                field[i] = null;
                while (field[i] == null)
                {
                    BlockType type;
                    if (!grayBlock &&
                        scrolledRows > grayBlockDelay &&
                        random.NextDouble() < grayBlockChance)
                    {
                        type = BlockType.Gray;
                    }
                    else
                    {
                        type = GetRandomBlockType(random);
                    }

                    // make sure block won't pop immediately

                    if (!IsLeftmost(i, 2) &&
                        IsOfType(field[LeftOf(i)], type) &&
                        IsOfType(field[LeftOf(i, 2)], type))
                        continue;

                    if (IsOfType(field[Above(i)], type) &&
                        IsOfType(field[Above(i, 2)], type))
                        continue;

                    grayBlock = grayBlock || (type == BlockType.Gray);
                    field[i] = new Block(type);
                }
            }
        }

        // TODO ugly static method
        public static BlockType GetRandomBlockType(Random random)
        {
            BlockType[] blockTypes = {  BlockType.Green,
                                        BlockType.Yellow,
                                        BlockType.Red,
                                        BlockType.Purple,
                                        BlockType.Cyan,
                                        BlockType.Blue
                                     };
            // TODO difficulty
            return blockTypes[random.Next(5)];
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
                    if (SwapBlocks(markerPos))
                        swapTimer = 0;
                    else
                        swapTimer--;
                }
                for (int i = 0; i < numBlocks; i++)
                {
                    if (field[i] != null)
                    {
                        field[i].Update();
                    }
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
                    if (!IsTopmostVisible(markerPos, GetHeight() >= visibleHeight ? 1 : 2))
                        markerPos = Above(markerPos);
                    break;

                case InputType.Left:
                    swapTimer = 0;
                    if (!IsLeftmost(markerPos))
                        markerPos = LeftOf(markerPos);
                    break;

                case InputType.Right:
                    swapTimer = 0;
                    if (!IsRightmost(markerPos, 2))
                        markerPos = RightOf(markerPos);
                    break;

                case InputType.Down:
                    swapTimer = 0;
                    if (!IsBottommostVisible(markerPos))
                        markerPos = Below(markerPos);
                    break;

                case InputType.Swap:
                    if (state != PlayFieldState.Play)
                        break;
                    if (!SwapBlocks(markerPos))
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

        private bool SwapBlocks(int pos)
        {
            if (IsForthcoming(pos))
                return false;

            Block left = field[pos];
            Block right = field[RightOf(pos)];

            if (left != null && (!IsOfState(left, BlockState.Idle) || IsGarbage(left)))
                return false;
            if (right != null && (!IsOfState(right, BlockState.Idle) || IsGarbage(right)))
                return false;

            if (left == null && right == null)
                return false;

            if (left == null)
            {
                if (IsHoverOrMove(field[Above(pos)]))
                    return false;

                if (IsOfState(field[RightOf(Above(pos))], BlockState.Idle))
                    field[RightOf(Above(pos))].Hover(9);
            }
            else if (right == null)
            {
                if (IsHoverOrMove(field[RightOf(Above(pos))]))
                    return false;

                if (IsOfState(field[Above(pos)], BlockState.Idle))
                    field[Above(pos)].Hover(9);
            }

            field[pos] = right;
            field[RightOf(pos)] = left;

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
            for (int i = 0; !IsForthcoming(i); i++)
            {
                if (field[i] == null)
                    continue;

                if (field[i].State != BlockState.Idle)
                {
                    fastScroll = false;
                    return false;
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
                    for (int i = 0; !IsForthcoming(i); i++)
                        field[i] = field[Below(i)];
                    RandomizeRow(height - 1);

                    if ((GetHeight() == firstVisibleRow) ||
                       (markerPos > firstBlockSecondVisibleRow))
                    {
                        markerPos -= width;
                    }

                    if (GetHeight() == firstVisibleRow)
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
            for (int i = width; !IsForthcoming(i); i++)
            {
                if (field[i] == null || field[i].State != BlockState.Dead)
                    continue;

                if(!IsGarbage(field[i]))
                {
                    Chain chain = field[i].Chain;
                    // Propagate chain upwards, and drop idle blocks
                    for (int y = Above(i); !IsTopmost(y) && IsDropable(field[y]); y = Above(y))
                    {
                        field[y].Chain = chain;
                        if (field[y].State == BlockState.Idle)
                            field[y].Drop();
                    }
                }
                field[i] = field[i].ReplaceBlock();
            }
        }

        private void DropBlocks()
        {
            //loop through field, starting at the bottom
            for (int i = lastBlockSecondLastRow; !IsTopmost(i); i--)
            {
                if (field[i] == null)
                    continue;

                if (field[i].State == BlockState.PostMove)
                {
                    if (field[Below(i)] == null)
                    {
                        //hover a bit before dropping  and don't pop
                        field[i].Hover(4);
                        field[i].PopChecked();
                        continue;
                    }

                    if (IsOfState(field[Above(i)], BlockState.Hover))
                    {
                        //if there's a block above, and it's hovering
                        //(which it will be if it dropped on this block while it was MOVING)

                        field[i].Land(); //land this block now instead of next tick

                        for (int y = Above(i); !IsTopmost(y) && IsOfState(field[y], BlockState.Hover); y = Above(y))
                            field[y].DropAndLand();
                    }
                    continue;
                }

                if (!IsBottommostVisible(i) && field[i].State == BlockState.Idle)
                {
                    if (field[Below(i)] == null)
                        field[i].Drop();
                    else if (field[Below(i)].State == BlockState.Falling)
                    {
                        field[i].Drop();
                        if (IsBlock(field[i]))
                            field[i].Chain = field[Below(i)].Chain;
                    }
                }

                if (field[i].State == BlockState.Falling)
                {
                    if (IsHoverOrMove(field[Below(i)]))
                    {
                        field[i].Hover(1500);//we hover this block a while
                        //(it won't hover this long though since
                        //it'll drop when the ones below drop
                        // BUG, sometimes it keeps hovering, why??
                        continue;

                    }
                    else if (field[Below(i)] != null && field[Below(i)].State != BlockState.Falling)
                    {
                        field[i].Land();
                        continue;
                    }
                    if (!IsTopmost(i) && IsHoverOrIdle(field[Above(i)]))
                    {
                        // This happens when a stack of blocks 'landed'
                        // on a block the player moved in below the stack,
                        // and it's time to start falling again (BST_HOVER),
                        // or when a block is pulled out of a row and the
                        // block above where it was has just stopped hovering (BST_IDLE)
                        field[Above(i)].Drop();
                    }
                }

                if (IsOfState(field[i], BlockState.Hover))
                    if (IsOfState(field[Below(i)], BlockState.Idle) || IsOfState(field[Below(i)], BlockState.Flash))
                        field[i].DropAndLand();

            }

            //loop through field, starting at the bottom
            for (int i = lastBlockSecondLastRow; !IsTopmost(i); i--)
            {
                if (IsOfState(field[i], BlockState.Falling))
                {
                    if (field[i].CheckDrop())	//if it's time to really drop the block
                    {
                        Debug.Assert(field[Below(i)] == null);
                        field[Below(i)] = field[i];
                        field[i] = null;
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

            for (int i = width; !IsForthcoming(i); i++)//loop, top to bottom
            {
                bClearChain = true;
                if (field[i] == null)//skip if there's no block
                    continue;

                if (field[i].State != BlockState.Falling)// no falling blocks please
                    if (fieldHeight[i % width] == -1)//if we haven't already found one in the same column
                        fieldHeight[i % width] = height - i / width - 1;//set height

                if (IsGarbage(field[i]) || !field[i].NeedPopCheck())
                {
                    if (field[i].State == BlockState.Pop2)//but we want to check if we need to 'pop' it
                    {
                        int chainlength = 0;
                        if (!IsGarbage(field[i]))
                        {
                            score += 10;
                            chainlength = field[i].Chain.length;
                        }
                        // TODO effect
                        Debug.WriteLine("Chain pop " + chainlength);
                        /*
				        if(IsVisible(i))
					        effects->Add(new EffPop(i, chainlength));
				        Sound::PlayPopEffect(field[i]->GetChain());
                        */
                    }
                    continue;
                }

                int top = i;
                for (; !IsTopmost(top); top = Above(top))
                {
                    if (!IsPopableWith(field[i], field[Above(top)]))
                        break;
                }

                tmpChain = null;

                int bottom = top;
                for (; !IsBottommostVisible(bottom); bottom = Below(bottom))
                {
                    // TODO: Move the chain thing after we know the block is used
                    if (tmpChain == null)
                        tmpChain = field[bottom].Chain; //if one of the blocks is in a chain
                    //this'll store that
                    if (!IsPopableWith(field[i], field[Below(bottom)]))
                        break;
                }

                if (bottom >= Below(top, 2)) // Distance between 3 adjacent blocks is 2
                {
                    for (int check = top; check <= bottom; check = Below(check))
                    {
                        if (!field[check].IsPopped())
                        {
                            field[check].Chain = tmpChain;
                            popper.AddBlock(field[check], check);
                            field[check].SetPop();
                        }
                    }
                    bClearChain = false;//and make sure we don't clear the chain
                    bPop = true;//and set popflag
                }

                int left = i;
                for (; !IsLeftmost(left); left = LeftOf(left))
                {
                    if (!IsPopableWith(field[i], field[LeftOf(left)]))
                        break;
                }

                tmpChain = null;

                int right = left;
                for (; !IsRightmost(right); right = RightOf(right))
                {
                    // TODO: Move until we know block should be in chain
                    if (tmpChain == null)
                        tmpChain = field[right].Chain;//store the chain

                    if (!IsPopableWith(field[i], field[RightOf(right)]))
                        break;
                }

                if (right >= RightOf(left, 2)) // Distance between 3 adjacent blocks is 2
                {
                    for (int check = left; check <= right; check = RightOf(check))
                    {
                        if (!field[check].IsPopped())
                        {
                            field[check].Chain = tmpChain;
                            popper.AddBlock(field[check], check);
                            field[check].SetPop();
                        }
                    }
                    bClearChain = false;
                    bPop = true;
                }

                field[i].PopChecked();
                if (bClearChain && field[i].State != BlockState.Hover)
                    field[i].Chain = null;
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
                    for (int i = lastBlockSecondLastRow; !IsTopmost(i); i--)//loop, bottom to top
                    {
                        // This checks if there are GarbageBlocks that needs to pop
                        // TODO: Do this in terms of the larger GarbageBlocks instead of the individual fields
                        if (!IsGarbage(field[i]) || field[i].State != BlockState.Idle)
                            continue;
                        if (field[i].IsPopped())
                            continue;

                        GarbageBlock g = (GarbageBlock)field[i];
                        tmpChain = SamePopChain(g, field[Below(i)], tmpChain);
                        tmpChain = SamePopChain(g, field[Above(i)], tmpChain);
                        if (!IsRightmost(i))
                            tmpChain = SamePopChain(g, field[RightOf(i)], tmpChain);
                        if (!IsLeftmost(i))
                            tmpChain = SamePopChain(g, field[LeftOf(i)], tmpChain);

                        if (!g.IsPopped()) // If it didn't pop, next!
                            continue;

                        if (tmpChain != null)
                        {
                            gh.AddPop(g.GB, tmpChain, bReverse);
                            bDoOver = true;
                            tmpChain = null;
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
            tooHigh = (GetHeight() >= visibleHeight);

            if (!tooHigh)
                dieTimer = 0;

            for (int i = 0; i < width; i++)
            {
                StressState stress =
                    (fieldHeight[i] >= stressHeight) ?
                        ((tooHigh || scrollPause > 0) ? StressState.Stop : StressState.Stress) :
                        StressState.Normal;
                for (int o = i; !IsForthcoming(o); o = Below(o))
                {
                    if (IsBlock(field[o]))
                        field[o].Stress = stress;
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

        private bool IsLineOfFieldEmpty(int x)
        {
            for (int i = 0; i < width; i++)
            {
                if (field[LeftOf(x, i)] != null)
                    return false;
            }
            return true;
        }

        public bool InsertGarbage(BigGarbageBlock b)
        {
            int pos = Math.Min(
                garbageDropStart,
                numBlocks - GetHeight() * width - 1);

            // Get the position where we should start inserting
            // Unfortunately GetHeight only counts normal blocks..
            while (pos >= 0 && !IsLineOfFieldEmpty(pos))
                pos = Above(pos);

            if (IsTopmost(pos - b.GetNum()))
            {
                // Out of space to drop blocks
                return false;
            }

            if (leftAlignGarbage && b.GetNum() < width)
                pos -= width - b.GetNum();
            for (int i = b.GetNum() - 1; i >= 0; i--, pos = LeftOf(pos))
            {
                Debug.Assert(field[pos] == null);
                field[pos] = b.GetBlock(i);
            }

            leftAlignGarbage = !leftAlignGarbage;

            return true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            offset.Y += (float)scrollOffset;
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

            // Draw blocks
            for (int i = firstBlockFirstVisibleRow; i < numBlocks; i++)
            {
                Block block = field[i];
                if (block != null)
                {
                    int tile = block.Tile;
                    Vector2 pos = PosToVector(i) + offset;
                    spriteBatch.Draw(
                        blocksTileSet.Texture,
                        new Rectangle(
                            (int)pos.X,
                            (int)pos.Y,
                            blockSize,
                            blockSize),
                        blocksTileSet.SourceRectangle(tile),
                        i >= firstBlockLastRow ? Color.DarkGray : Color.White);
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

        private int RightOf(int i, int amount = 1)
        {
            return i + 1 * amount;
        }
        private int LeftOf(int i, int amount = 1)
        {
            return i - 1 * amount;
        }
        private int Above(int i, int amount = 1)
        {
            return i - width * amount;
        }
        private int Below(int i, int amount = 1)
        {
            return i + width * amount;
        }
        private bool IsLeftmost(int i, int amount = 1)
        {
            return i % width < amount;
        }
        private bool IsRightmost(int i, int amount = 1)
        {
            return i % width >= width - amount;
        }
        private bool IsTopmostVisible(int pos, int amount = 1)
        {
            return Above(pos, amount) < firstBlockFirstVisibleRow;
        }
        private bool IsBottommostVisible(int pos, int amount = 1)
        {
            return Below(pos, amount) >= firstBlockLastRow;
        }
        private bool IsVisible(int pos)
        {
            return (pos >= firstBlockFirstVisibleRow);
        }
        private bool IsTopmost(int pos)
        {
            return pos < width;
        }
        private bool IsForthcoming(int pos)
        {
            return IsBottommostVisible(pos, 0);
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

        public Vector2 PosToVector(int pos)
        {
            return new Vector2(
                (pos % width) * blockSize,
                (pos / width - firstVisibleRow) * blockSize);
        }

        public void ActivatePerformedCombo(int pos, bool isChain, int count)
        {
            ComboEventArgs eventArgs = new ComboEventArgs(
                PosToVector(pos),
                isChain,
                count);
            PerformedCombo(this, eventArgs);
        }

        public event EventHandler<ComboEventArgs> PerformedCombo;
    }
}
