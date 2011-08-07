using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Media
{
    public class Song
    {
        internal SoundEffect effect;

        Song(SoundEffect effect)
        {
            this.effect = effect;
        }

        internal static Song _FromWavStream(Stream stream)
        {
            return new Song(SoundEffect._FromWavStream(stream));
        }
    }
}

