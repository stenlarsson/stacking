using System;
using Tao.Sdl;
using Tao.OpenAl;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Audio
{
    public class SoundEffectInstance : IDisposable
    {
        private int source;

        internal SoundEffectInstance(SoundEffect effect)
        {
            Al.alGenSources(1, out source);
            if (Al.alGetError() != Al.AL_NO_ERROR)
                throw new Exception(Alut.alutGetErrorString(Al.alGetError()));

            Al.alSourcei(source, Al.AL_BUFFER, effect.buffer);
        }

        ~SoundEffectInstance() {
            Dispose();
        }

        public void Dispose()
        {
            Al.alDeleteSources(1, ref source);
            source = Al.AL_NONE;
        }

        public virtual bool IsLooped {
            get {
                int result;
                Al.alGetSourcei(source, Al.AL_LOOPING, out result);
                return result == Al.AL_TRUE;
            }
            set {
                Al.alSourcei(source, Al.AL_LOOPING, value ? Al.AL_TRUE : Al.AL_FALSE);
            }
        }
        public float Volume {
            get {
                float result;
                Al.alGetSourcef(source, Al.AL_GAIN, out result);
                return result;
            }
            set {
                Al.alSourcef(source, Al.AL_GAIN, Math.Max(Math.Min(value,1.0f),0.0f));
            }
        }
        public float Pitch {
            get {
                float result;
                Al.alGetSourcef(source, Al.AL_PITCH, out result);
                return (result < 1) ? (result-1)*2 : result-1;
            }
            set {
                Al.alSourcef(source, Al.AL_PITCH, value < 0 ? 1+value/2 : 1+value);
            }
        }
        public float Pan {
            get {
                return 0.0f;
            }
            set {
                if (value != 0.0f) {
                    throw new NotImplementedException();
                }
            }
       }

        public void Play()
        {
            Al.alSourcePlay(source);
            if (Al.alGetError() != Al.AL_NO_ERROR)
                throw new Exception(Alut.alutGetErrorString(Al.alGetError()));
        }
    }

    public sealed class SoundEffect : IDisposable
    {
        internal int buffer;

        private List<SoundEffectInstance> instances = new List<SoundEffectInstance>();

        internal SoundEffect (string file)
        {
            buffer = Alut.alutCreateBufferFromFile(file);
            if (buffer == Al.AL_NONE) {
                throw new Exception(Alut.alutGetErrorString(Alut.alutGetError()));
            }
        }

        public void Dispose()
        {
            foreach (SoundEffectInstance i in instances) {
                i.Dispose();
            }
            instances.Clear();
            Al.alDeleteBuffers(1, ref buffer);
        }

        public void Play()
        {
            Play(1.0f, 1.0f, 0.0f);
        }

        public void Play(float volume, float pitch, float pan)
        {
            // TODO: Remove/Dispose all finished instances...
            // TODO: Reuse source instead of freeing and reallocating
            SoundEffectInstance i = new SoundEffectInstance(this){
                Volume = volume,
                Pitch = pitch,
                Pan = pan
            };
            instances.Add(i);
            i.Play();
        }

        public SoundEffectInstance CreateInstance()
        {
            return new SoundEffectInstance(this);
        }
    }
}

