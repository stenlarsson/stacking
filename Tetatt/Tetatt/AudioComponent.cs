using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Tetatt.GamePlay;

namespace Tetatt
{
    class AudioComponent : GameComponent
    {
        const int MusicChangeDelay = 20;

        SoundEffect[] popEffect;
        SoundEffect chainEffect;
        SoundEffect fanfare1Effect;
        SoundEffect fanfare2Effect;

        SoundEffect normalMusic;
        SoundEffect stressMusic;
        SoundEffectInstance music;
        int musicChangeTimer;
        bool isStressMusic;

        List<PlayField> playFields;

        /// <summary>
        /// Create a new AudioComponent and add to game services.
        /// </summary>
        public AudioComponent(Game game)
            : base(game)
        {
            playFields = new List<PlayField>();

            game.Services.AddService(typeof(AudioComponent), this);
        }

        /// <summary>
        /// Load music and sound effects
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            ContentManager content = Game.Content;

            // Load sound effects
            popEffect = new SoundEffect[4];
            for (int i = 0; i < popEffect.Length; i++)
            {
                popEffect[i] = content.Load<SoundEffect>("pop" + (i + 1));
            }
            chainEffect = content.Load<SoundEffect>("chain");
            fanfare1Effect = content.Load<SoundEffect>("fanfare1");
            fanfare2Effect = content.Load<SoundEffect>("fanfare2");

            // Load music
            normalMusic = content.Load<SoundEffect>("normal_music");
            stressMusic = content.Load<SoundEffect>("stress_music");
        }

        /// <summary>
        /// Switch to stressful music if anyone reaches a certain height, or back if
        /// everyone is below again. Use a delay to avoid changing too often.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (music != null)
            {
                bool anyStress = false;
                foreach (var playField in playFields)
                {
                    if (playField.GetHeight() >= PlayField.stressHeight)
                    {
                        anyStress = true;
                        break;
                    }
                }
                if (anyStress != isStressMusic && --musicChangeTimer <= 0)
                {
                    music.Dispose();
                    music = (anyStress ? stressMusic : normalMusic).CreateInstance();
                    music.IsLooped = true;
                    music.Play();
                    isStressMusic = anyStress;
                    musicChangeTimer = MusicChangeDelay;
                }
            }
        }

        /// <summary>
        /// Play sound effect for supplied play field
        /// </summary>
        public void AddPlayField(PlayField playField)
        {
            playFields.Add(playField);
            playField.PerformedCombo += PerformedCombo;
            playField.PerformedChain += PerformedChain;
            playField.Popped += Popped;
            playField.Died += Died;
        }

        /// <summary>
        /// Stop playing sound effects for supplied play field.
        /// </summary>
        public void RemovePlayField(PlayField playField)
        {
            playFields.Remove(playField);
            playField.PerformedCombo -= PerformedCombo;
            playField.PerformedChain -= PerformedChain;
            playField.Popped -= Popped;
            playField.Died -= Died;
        }

        /// <summary>
        /// Start playing music
        /// </summary>
        public void GameStarted()
        {
            music = normalMusic.CreateInstance();
            music.IsLooped = true;
            music.Play();
            isStressMusic = false;
            musicChangeTimer = MusicChangeDelay;
        }

        /// <summary>
        /// Stop playing music
        /// </summary>
        public void GameEnded()
        {
            if (music != null)
            {
                music.Dispose();
                music = null;
            }
        }

        /// <summary>
        /// Stop music and stop playing sound effects for all play fields
        /// </summary>
        public void Reset()
        {
            if (music != null)
            {
                music.Dispose();
                music = null;
            }

            // Loop over a copy of the list. Cannot modify a list while iterating over it.
            foreach (var playField in new List<PlayField>(playFields))
            {
                RemovePlayField(playField);
            }
        }

        /// <summary>
        /// Called when a playfield performed a combo or a step in a chain 
        /// </summary>
        private void PerformedCombo(PlayField sender, Pos pos, bool isChain, int count)
        {
            if (isChain)
            {
                chainEffect.Play();
            }
        }

        /// <summary>
        /// Called when a chain is completed
        /// </summary>
        private void PerformedChain(PlayField sender, Chain chain)
        {
            if (chain.length == 4)
            {
                fanfare1Effect.Play();
            }
            else if (chain.length > 4)
            {
                fanfare2Effect.Play();
            }
        }
        
        /// <summary>
        /// Called when a block is popped
        /// </summary>
        private void Popped(PlayField sender, Pos pos, bool isGarabge, Chain chain)
        {
            SoundEffect effect = popEffect[Math.Min(chain.length, 4) - 1];
            effect.Play(1, chain.popCount / 10.0f, 0);

            if (chain.popCount < 10)
            {
                chain.popCount++;
            }
        }

        /// <summary>
        /// Called when a playfield dies
        /// </summary>
        private void Died(PlayField sender)
        {
        }
    }
}
