using System;
using System.Runtime.Serialization;

namespace Tetatt.ArtificialIntelligence
{
    [Serializable]
    public sealed class Level : ISerializable
    {
        public static readonly Level Easy = new Level(0, Resources.Easy);
        public static readonly Level Normal = new Level(1, Resources.Normal);
        public static readonly Level Hard = new Level(2, Resources.Hard);
        public static readonly Level[] All = { Easy, Normal, Hard };

        internal readonly int Number;
        public readonly string Name;

        public Level(int number, string name)
        {
            this.Number = number;
            this.Name = name;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(LevelSerializationHelper));
            info.AddValue("number", Number);
        }
    }

    [Serializable]
    sealed class LevelSerializationHelper : IObjectReference
    {
#pragma warning disable 0649
        int number;
#pragma warning restore 0649

        public object GetRealObject(StreamingContext context)
        {
            return Level.All[number];
        }
    }
}
