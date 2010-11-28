using System;
using Tao.Sdl;

namespace Microsoft.Xna.Framework.Audio
{
    public class SoundEffectInstance : IDisposable
    {
        private SoundEffect effect;
        public virtual bool IsLooped { get; set; }

        public void Dispose()
        {
        }

        internal SoundEffectInstance(SoundEffect effect)
        {
            this.effect = effect;
        }

        public void Play()
        {
            if (SdlMixer.Mix_PlayChannel(-1, effect.chunk, IsLooped ? -1 : 0) < 0)
            {
                Console.WriteLine("Failed playing sound effect");
            }
        }
    }

    public sealed class SoundEffect : IDisposable
    {
        internal IntPtr chunk;

        internal SoundEffect (string file)
        {
            chunk = SdlMixer.Mix_LoadWAV(file);
        }

        public void Dispose()
        {
            SdlMixer.Mix_FreeChunk(chunk);
        }

        public void Play()
        {
            CreateInstance().Play();
        }

        public SoundEffectInstance CreateInstance()
        {
            return new SoundEffectInstance(this);
        }
    }
}

