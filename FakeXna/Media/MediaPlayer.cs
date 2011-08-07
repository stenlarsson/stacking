using System;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Media
{
    public sealed class MediaPlayer
    {
        static MediaPlayer()
        {
            Queue = new MediaQueue();
        }

        static SoundEffectInstance playing = null;

        public static MediaQueue Queue { get; private set; }

        public static bool IsRepeating
        {
            get {
                if (playing == null)
                    return false;
                return playing.IsLooped;
            }
            set {
                if (playing == null)
                    throw new NotImplementedException();
                playing.IsLooped = value;
            }
        }

        public static MediaState State
        {
            get {
                if (playing == null)
                    return MediaState.Stopped;
                return (MediaState)(int)playing.State;
            }
        }

        public static void Play(Song song)
        {
            Stop();

            playing = song.effect.CreateInstance();
            Queue.ActiveSong = song;
            playing.Play();
        }

        public static void Stop()
        {
            if (playing != null)
            {
                playing.Dispose();
                playing = null;
            }
            Queue.ActiveSong = null;
        }
    }
}

