using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.GamePlay;

namespace Tetatt.ArtificialIntelligence
{
    class AIPlayer
    {
        /// <summary>
        /// Parameters used on each stage. Currently only contains
        /// the InputDelay.
        /// </summary>
        public readonly int[,] Stages = new int[,] {
            {60, 59, 58, 57, 56, 55, 54, 53, 52, 51},
            {30, 29, 28, 27, 26, 25, 24, 23, 22, 21},
            {10,  9,  8,  7,  6,  5,  4,  3,  2,  1},
        };

        /// <summary>
        /// Score for making three blocks pop
        /// </summary>
        const float PopScore = 40;
        /// <summary>
        /// Score for making a block drop down
        /// </summary>
        const float FlattenScore = 20;
        /// <summary>
        /// Multiplier for making a chain
        /// </summary>
        const float ChainMultiplier = 2;
        /// <summary>
        /// Multiplier for making a garbage chain
        /// </summary>
        const float ClearGarbageMultiplier = 2;
        /// <summary>
        /// Raise if field, counting garbage, is lower than this height
        /// </summary>
        const int RaiseHeight = 12;
        /// <summary>
        /// Raise if field, not counting garbage, is lower than this height
        /// </summary>
        const int RaiseHeightWithoutGarbage = 8;
        /// <summary>
        /// Number of frames between each input
        /// </summary>
        int InputDelay = 1;

        int inputDelayTimer;

        PlayField playField;

        /// <summary>
        /// Create new AI player controlling specified field.
        /// </summary>
        public AIPlayer(PlayField playField)
        {
            this.playField = playField;
        }

        /// <summary>
        /// Set difficulty according to stage
        /// </summary>
        public void SetDifficulty(int level, int stage)
        {
            // TODO RaiseHeight and other values should also be modified
            InputDelay = Stages[level, stage];
        }

        /// <summary>
        /// Return the input the AI wants to perform this frame
        /// </summary>
        public PlayerInput GetInput()
        {
            // Only make a move every InputDelay frames
            if (inputDelayTimer > 0)
            {
                // TODO This is too crude. A player can hold down button to
                // make a skill chain, and the AI can't.
                inputDelayTimer--;
                return PlayerInput.None;
            }
            else
            {
                inputDelayTimer = InputDelay;
            }

            SimplifiedPlayField sim = new SimplifiedPlayField(playField);

            sim.Pop();

            int height;
            int heightWithoutGarbage;

            sim.Height(out height, out heightWithoutGarbage);
            if (sim.CanRaise && height < RaiseHeight && heightWithoutGarbage < RaiseHeightWithoutGarbage)
            {
                return PlayerInput.Raise;
            }

            float swapScore;
            Pos bestSwap = CalculateBestSwap(sim, out swapScore);

            if (bestSwap.Row > playField.markerPos.Row)
            {
                return PlayerInput.Up;
            }
            else if (bestSwap.Row < playField.markerPos.Row)
            {
                return PlayerInput.Down;
            }
            else if (bestSwap.Col < playField.markerPos.Col)
            {
                return PlayerInput.Left;
            }
            else if (bestSwap.Col > playField.markerPos.Col)
            {
                return PlayerInput.Right;
            }
            else if (swapScore > 0)
            {
                return PlayerInput.Swap;
            }
            else
            {
                return PlayerInput.None;
            }
        }

        /// <summary>
        /// Calculate where it is most benficial to input a swap, considering the number of
        /// moves needed to get there.
        /// </summary>
        public Pos CalculateBestSwap(SimplifiedPlayField sim, out float result)
        {
            SortedDictionary<float, Pos> moves = new SortedDictionary<float, Pos>();

            // Each row except the top and bottom rows, they can't be reached
            for (int row = 1; row < sim.Field.GetLength(0) - 1; row++)
            {
                // Each column except the rightmost
                for (int col = 0; col < sim.Field.GetLength(1) - 1; col++)
                {
                    float score = 0;
                    float scoreLeft = 0;
                    float scoreRight = 0;

                    if (!sim.CanSwap(row, col))
                    {
                        continue;
                    }

                    SimplifiedPlayField simcpy = new SimplifiedPlayField(sim);
                    simcpy.Swap(row, col);
                    score += CalculateScore(simcpy, row, col);

                    // Check score if we also swap one piece to the left
                    if (col > 0)
                    {
                        simcpy = new SimplifiedPlayField(sim);
                        simcpy.Swap(row, col);
                        simcpy.Settle();
                        if (simcpy.CanSwap(row, col - 1))
                        {
                            simcpy.Swap(row, col - 1);
                            scoreLeft += CalculateScore(simcpy, row, col - 1) - 2;
                        }
                    }

                    // Check score if we also swap one piece to the right
                    if (col < sim.Field.GetLength(1) - 2)
                    {
                        simcpy = new SimplifiedPlayField(sim);
                        simcpy.Swap(row, col);
                        simcpy.Settle();
                        if (simcpy.CanSwap(row, col + 1))
                        {
                            simcpy.Swap(row, col + 1);
                            scoreRight += CalculateScore(simcpy, row, col + 1) - 2;
                        }
                    }

                    // Take the best score
                    score = Math.Max(score, Math.Max(scoreLeft, scoreRight));

                    // Penalty for moving cursor so that in case of equally good
                    // moves we take the nearest one. (Note: Can't move diagonally)
                    int distance =
                        Math.Abs(playField.markerPos.Row - row) +
                        Math.Abs(playField.markerPos.Col - col);
                    score -= distance;

                    if (!moves.ContainsKey(score))
                    {
                        moves.Add(score, new Pos(row, col));
                    }
                }
            }

            foreach (var items in moves.Reverse())
            {
                result = items.Key;
                return items.Value;
            }

            throw new Exception("Supposedly unreachable code");
        }

        /// <summary>
        /// Calculate the score for making a swap at the specified position
        /// </summary>
        public float CalculateScore(SimplifiedPlayField sim, int row, int col)
        {
            float score = 0;

            // If swapping causes pieces to drop down it will flatten
            // the play field which is a good thing
            // Note that we have already swapped when getting here
            if (sim.Field[row, col].Type.HasValue &&
                sim.Field[row - 1, col + 1].Type.HasValue &&
                !sim.Field[row, col + 1].Type.HasValue &&
                !sim.Field[row - 1, col].Type.HasValue ||
                sim.Field[row, col + 1].Type.HasValue &&
                sim.Field[row - 1, col].Type.HasValue &&
                !sim.Field[row, col].Type.HasValue &&
                !sim.Field[row - 1, col + 1].Type.HasValue)
            {
                score += FlattenScore;
            }

            // Let pices fall down
            sim.Settle();

            for (row = 1; row < sim.Field.GetLength(0); row++)
            {
                for (col = 0; col < sim.Field.GetLength(1) ; col++)
                {
                    // TODO If there are 4 in a row we will get another PopScore in
                    // next iteration, resulting in PopScore*2 for 4 pieces. If both
                    // this and the below test are true it will result in
                    // PopScore*2 for 5 pieces...

                    if (col < sim.Field.GetLength(1) - 2 &&
                        sim.Field[row, col].Type.HasValue &&
                        sim.Field[row, col].Type != BlockType.Garbage &&
                        sim.Field[row, col].Type == sim.Field[row, col + 1].Type &&
                        sim.Field[row, col].Type == sim.Field[row, col + 2].Type)
                    {
                        score += PopScore;

                        if (sim.Field[row, col].InChain ||
                            sim.Field[row, col + 1].InChain ||
                            sim.Field[row, col + 2].InChain)
                        {
                            score *= ChainMultiplier;
                        }

                        if (row < sim.Field.GetLength(0) - 1 && 
                            sim.Field[row + 1, col].Type == BlockType.Garbage ||
                            sim.Field[row + 1, col + 1].Type == BlockType.Garbage ||
                            sim.Field[row + 1, col + 2].Type == BlockType.Garbage)
                        {
                            score *= ClearGarbageMultiplier;
                        }
                    }

                    if (row < sim.Field.GetLength(0) - 2 &&
                        sim.Field[row, col].Type.HasValue &&
                        sim.Field[row, col].Type != BlockType.Garbage &&
                        sim.Field[row, col].Type == sim.Field[row + 1, col].Type &&
                        sim.Field[row, col].Type == sim.Field[row + 2, col].Type)
                    {
                        score += PopScore;

                        if (sim.Field[row, col].InChain ||
                            sim.Field[row + 1, col].InChain ||
                            sim.Field[row + 2, col].InChain)
                        {
                            score *= ChainMultiplier;
                        }

                        if (row < sim.Field.GetLength(0) - 3 &&
                            sim.Field[row + 3, col].Type == BlockType.Garbage)
                        {
                            score *= ClearGarbageMultiplier;
                        }
                    }
                }
            }

            return score;
        }
    }
}
