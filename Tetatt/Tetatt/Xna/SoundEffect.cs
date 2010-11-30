using System;
using Tao.Sdl;
using Tao.OpenAl;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Audio
{
    public enum SoundState
    {
        Paused, Playing, Stopped
    }

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
            if (source != Al.AL_NONE)
            {
                GC.SuppressFinalize(this);
                Al.alDeleteSources(1, ref source);
                source = Al.AL_NONE;
            }
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
                Al.alSourcef(source, Al.AL_GAIN, MathHelper.Clamp(value, 0.0f, 1.0f));
            }
        }
        public float Pitch {
            get {
                float result;
                Al.alGetSourcef(source, Al.AL_PITCH, out result);
                return MathHelper.Clamp((float)Math.Log( result, 2.0 ), -1.0f, 1.0f);
            }
            set {
                Al.alSourcef(source, Al.AL_PITCH, (float)Math.Pow(2, MathHelper.Clamp(value, -1.0f, 1.0f)));
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
        public SoundState State {
            get {
                int result;
                Al.alGetSourcei(source, Al.AL_SOURCE_STATE, out result);
                switch (result) {
                case Al.AL_PLAYING:
                    return SoundState.Playing;
                case Al.AL_PAUSED:
                    return SoundState.Paused;
                case Al.AL_INITIAL:
                case Al.AL_STOPPED:
                    return SoundState.Stopped;
                default:
                    throw new NotSupportedException();
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

        private Queue<SoundEffectInstance> instances = new Queue<SoundEffectInstance>();

        internal SoundEffect (string file)
        {
            buffer = Alut.alutCreateBufferFromFile(file);
            if (buffer == Al.AL_NONE) {
                throw new Exception(Alut.alutGetErrorString(Alut.alutGetError()));
            }
            instances.Enqueue(new SoundEffectInstance(this));
        }

        ~SoundEffect()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (buffer != Al.AL_NONE)
            {
                GC.SuppressFinalize(this);
                foreach (SoundEffectInstance i in instances)
                {
                    i.Dispose();
                }
                instances.Clear();
                Al.alDeleteBuffers(1, ref buffer);
                buffer = Al.AL_NONE;
            }
        }

        public void Play()
        {
            Play(1.0f, 1.0f, 0.0f);
        }

        public void Play(float volume, float pitch, float pan)
        {
            // Either reuse existing, or create new sound effect
            SoundEffectInstance i = instances.Peek();
            if (i.State == SoundState.Stopped)
            {
                instances.Dequeue();
            }
            else
            {
                i = new SoundEffectInstance(this);
            }

            i.Volume = volume;
            i.Pitch = pitch;
            i.Pan = pan;
            i.Play();

            instances.Enqueue(i);
        }

        public SoundEffectInstance CreateInstance()
        {
            return new SoundEffectInstance(this);
        }
    }
}

