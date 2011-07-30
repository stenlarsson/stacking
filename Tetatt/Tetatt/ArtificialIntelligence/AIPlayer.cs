using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tetatt.GamePlay;

namespace Tetatt.ArtificialIntelligence
{
    class AIPlayer
    {
        const float PopScore = 40;
        const float FlattenScore = 20;
        const float ChainMultiplier = 2;
        const int RaiseHeight = 8;

        PlayField playField;
        Random random;

        public AIPlayer(PlayField playField)
        {
            this.playField = playField;
            random = new Random();
        }

        public PlayerInput GetInput()
        {
            SimplifiedPlayField sim = new SimplifiedPlayField(playField);

            if (sim.Height() < RaiseHeight)
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
            else if (swapScore > CalculateScore(sim))
            {
                return PlayerInput.Swap;
            }
            else
            {
                return PlayerInput.None;
            }
        }

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

                    if (!sim.CanSwap(row, col))
                    {
                        continue;
                    }

                    // If swapping causes pieces to drop down it will flatten
                    // the play field which is a good thing
                    if (sim.Field[row, col].Type.HasValue &&
                        !sim.Field[row, col + 1].Type.HasValue &&
                        !sim.Field[row - 1, col + 1].Type.HasValue ||
                        sim.Field[row, col + 1].Type.HasValue &&
                        !sim.Field[row, col].Type.HasValue &&
                        !sim.Field[row - 1, col].Type.HasValue)
                    {
                        score += FlattenScore;
                    }

                    // Make a copy of the play field and swap to see what happens
                    SimplifiedPlayField simcpy = new SimplifiedPlayField(sim);
                    simcpy.Swap(row, col);

                    // Let pices fall down
                    simcpy.Settle();

                    // Can't move diagonally
                    int distance =
                        Math.Abs(playField.markerPos.Row - row) +
                        Math.Abs(playField.markerPos.Col - col);

                    score += CalculateScore(simcpy);
                    if (simcpy.Field[row, col].Fallen ||
                        simcpy.Field[row, col + 1].Fallen)
                    {
                        score *= ChainMultiplier;
                    }

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

        public float CalculateScore(SimplifiedPlayField sim)
        {
            float score = 0;

            for (int row = 0; row < sim.Field.GetLength(0); row++)
            {
                for (int col = 0; col < sim.Field.GetLength(1) ; col++)
                {
                    // TODO If there are 4 in a row we will get another PopScore in
                    // next iteration, resulting in PopScore*2 for 4 pieces. If both
                    // this and the below test are true it will result in
                    // PopScore*2 for 5 pieces...

                    if (col < sim.Field.GetLength(1) - 2 &&
                        sim.Field[row, col].Type.HasValue &&
                        sim.Field[row, col].Type == sim.Field[row, col + 1].Type &&
                        sim.Field[row, col].Type == sim.Field[row, col + 2].Type)
                    {
                        score += PopScore;
                    }

                    if (row < sim.Field.GetLength(0) - 2 &&
                        sim.Field[row, col].Type.HasValue &&
                        sim.Field[row, col].Type == sim.Field[row + 1, col].Type &&
                        sim.Field[row, col].Type == sim.Field[row + 2, col].Type)
                    {
                        score += PopScore;
                    }
                }
            }

            return score;
        }
    }
}
