using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Media
{
    public class Song
    {
        internal SoundEffect effect = null;

        internal Song(string path)
        {
            effect = new SoundEffect(path);
        }
    }
}

