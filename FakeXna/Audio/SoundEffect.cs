using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Audio.OpenAL;
using FakeXna.FFmpeg.AVCodec;
using FakeXna.FFmpeg.AVFormat;
using FakeXna.FFmpeg.AVUtil;
using Stream = System.IO.Stream;

namespace Microsoft.Xna.Framework.Audio
{
    public sealed class SoundEffect : IDisposable
    {
        internal int buffer;

        Queue<SoundEffectInstance> instances = new Queue<SoundEffectInstance>();

        // Loads a wave/riff audio file.
        static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                /*int riff_chunck_size =*/ reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                /*int format_chunk_size =*/ reader.ReadInt32();
                /*int audio_format =*/ reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                /*int byte_rate =*/ reader.ReadInt32();
                /*int block_align =*/ reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                /*int data_chunk_size =*/ reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        SoundEffect (byte [] sound_data, int length, ALFormat format, int sample_rate)
        {
            buffer = AL.GenBuffer();
            CheckAL();
            AL.BufferData(buffer, format, sound_data, length, sample_rate);
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

        internal static SoundEffect _FromWavStream(Stream stream)
        {
            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = LoadWave(stream, out channels, out bits_per_sample, out sample_rate);
            return
                new SoundEffect(
                     sound_data,
                     sound_data.Length,
                     GetSoundFormat(channels, bits_per_sample),
                     sample_rate);
        }

        internal static SoundEffect _FromGenericStream(Stream stream)
        {
            FormatContext.Init();
            Codec.Init();

            using (var ms = new MemoryStream())
            using (var input = new FormatContext(IOContext.FromStream(stream)))
            {
                Codec decoder;
                var context = input.FindBestStream(MediaType.Audio, out decoder).Codec;
                context.Open(decoder);

                byte[] buffer = new byte[CodecContext.MinimumOutputBufferSize];
                foreach (var packet in input.Frames)
                {
                    int bytes = context.DecodeAudio(buffer, packet);
                    ms.Write(buffer, 0, bytes);
                }

                if (context.Channels > 2)
                    throw new NotSupportedException();

                bool stereo = context.Channels == 2;
                ALFormat format;
                switch (context.SampleFormat)
                {
                case SampleFormat.Unsigned8:
                    format = stereo ? ALFormat.Stereo8 : ALFormat.Mono8; break;
                case SampleFormat.Signed16:
                    format = stereo ? ALFormat.Stereo16 : ALFormat.Mono16; break;
                default:
                    throw new NotSupportedException();
                }

                return new SoundEffect(ms.GetBuffer(), (int)ms.Length, format, context.SampleRate);
            }
        }
    }
}

