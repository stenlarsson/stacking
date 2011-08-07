using System;
namespace Microsoft.Xna.Framework.Media
{
    public sealed class MediaQueue
    {
        public MediaQueue()
        {
        }

        public Song ActiveSong { get; internal set; }
    }
}

