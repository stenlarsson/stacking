using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Media
{
    public class Song
    {
        internal SoundEffect effect;

        internal Song(string path)
        {
            effect = new SoundEffect(path);
        }
    }
}

