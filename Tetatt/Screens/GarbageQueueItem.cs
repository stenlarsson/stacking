
namespace Tetatt.GamePlay
{
    class GarbageQueueItem
    {
        public readonly int Time;
        public readonly int Size;
        public readonly GarbageType Type;

        public GarbageQueueItem(int time, int size, GarbageType type)
        {
            Time = time;
            Size = size;
            Type = type;
        }
    }
}
