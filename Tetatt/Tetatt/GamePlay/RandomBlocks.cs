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

        private readonly static Random random = new Random();

        public static BlockType Next(double grayBlockChance = 0.0, int kindCount = 5)
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

