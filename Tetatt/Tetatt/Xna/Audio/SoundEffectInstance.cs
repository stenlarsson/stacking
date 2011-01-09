using System;
using OpenTK.Audio.OpenAL;

namespace Microsoft.Xna.Framework.Audio
{
    public class SoundEffectInstance : IDisposable
    {
        private int source;

        internal SoundEffectInstance(SoundEffect effect)
        {
            source = AL.GenSource();
            SoundEffect.CheckAL();
            AL.Source(source, ALSourcei.Buffer, effect.buffer);
            SoundEffect.CheckAL();
        }

        ~SoundEffectInstance() {
            Dispose();
        }

        public void Dispose()
        {
            if (source != 0)
            {
                GC.SuppressFinalize(this);
                AL.DeleteSource(source);
                source = 0;
            }
        }

        public virtual bool IsLooped {
            get {
                bool result;
                AL.GetSource(source, ALSourceb.Looping, out result);
                SoundEffect.CheckAL();
                return result;
            }
            set {
                AL.Source(source, ALSourceb.Looping, value);
                SoundEffect.CheckAL();
            }
        }
        public float Volume {
            get {
                float result;
                AL.GetSource(source, ALSourcef.Gain, out result);
                SoundEffect.CheckAL();
                return result;
            }
            set {
                AL.Source(source, ALSourcef.Gain, MathHelper.Clamp(value, 0.0f, 1.0f));
                SoundEffect.CheckAL();
            }
        }
        public float Pitch {
            get {
                float result;
                AL.GetSource(source, ALSourcef.Pitch, out result);
                SoundEffect.CheckAL();
                return MathHelper.Clamp((float)Math.Log( result, 2.0 ), -1.0f, 1.0f);
            }
            set {
                AL.Source(source, ALSourcef.Pitch, (float)Math.Pow(2, MathHelper.Clamp(value, -1.0f, 1.0f)));
                SoundEffect.CheckAL();
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
                AL.GetSource(source, ALGetSourcei.SourceState, out result);
                switch ((ALSourceState)result) {
                case ALSourceState.Playing:
                    return SoundState.Playing;
                case ALSourceState.Paused:
                    return SoundState.Paused;
                case ALSourceState.Initial:
                case ALSourceState.Stopped:
                    return SoundState.Stopped;
                default:
                    throw new NotSupportedException();
                }
            }
        }

        public void Play()
        {
            AL.SourcePlay(source);
            SoundEffect.CheckAL();
        }

        public void Stop()
        {
            AL.SourceStop(source);
            SoundEffect.CheckAL();
        }
    }
}
