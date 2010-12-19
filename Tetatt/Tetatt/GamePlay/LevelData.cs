using System;

namespace Tetatt.GamePlay
{
    public class LevelData
    {
        public double scrollSpeed;
        public int numBlockTypes;
        public int flashTime;
        public int popStartOffset;
        public int popTime;
        public int effComboDuration;

        public LevelData(double scrollSpeed, int numBlockTypes, int flashTime, int popStartOffset, int popTime, int effComboDuration)
        {
            this.scrollSpeed = scrollSpeed;
            this.numBlockTypes = numBlockTypes;
            this.flashTime = flashTime;
            this.popStartOffset = popStartOffset;
            this.popTime = popTime;
            this.effComboDuration = effComboDuration;
        }
    }
}
