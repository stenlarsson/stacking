using System;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Collections.Generic;
using System.IO;

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

    public sealed class SoundEffect : IDisposable
    {
        internal int buffer;

        private Queue<SoundEffectInstance> instances = new Queue<SoundEffectInstance>();

        // Loads a wave/riff audio file.
        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        internal SoundEffect (string file)
        {
            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = LoadWave(File.Open(file, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            buffer = AL.GenBuffer();
            CheckAL();
            AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);
            CheckAL();
            instances.Enqueue(new SoundEffectInstance(this));
        }

        ~SoundEffect()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (buffer != 0)
            {
                GC.SuppressFinalize(this);
                foreach (SoundEffectInstance i in instances)
                {
                    i.Dispose();
                }
                instances.Clear();
                AL.DeleteBuffers(1, ref buffer);
                buffer = 0;
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

        internal static void CheckAL()
        {
            ALError error = AL.GetError();
            if (error != ALError.NoError)
                throw new Exception(AL.GetErrorString(error));
        }
    }
}

