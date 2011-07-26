using System;
namespace Tetatt.GamePlay
{
    public class RandomBlocks
    {
        private readonly static BlockType[] blockTypes = {
            BlockType.Blue,
            BlockType.Green,
            BlockType.Yellow,
            BlockType.Red,
            BlockType.Purple,
            BlockType.Cyan,
        };

        private Random random;
        private int kindCount;

        public RandomBlocks(int seed, int kindCount)
        {
            random = new Random(seed);
            this.kindCount = kindCount;
        }

        public BlockType Next(double grayBlockChance)
        {
            if (grayBlockChance > 0.0 && random.NextDouble() < grayBlockChance)
            {
                return BlockType.Gray;
            }
            else
            {
                return blockTypes[random.Next(kindCount)];
            }
        }
    }
}

