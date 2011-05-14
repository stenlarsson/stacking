using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Tetatt.GamePlay;
using Tetatt.Graphics;

namespace Tetatt
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TetattGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SoundEffect[] popEffect;
        SoundEffect chainEffect;
        SoundEffect fanfare1Effect;
        SoundEffect fanfare2Effect;

        PlayField[] playFields;

        SoundEffect normalMusic;
        SoundEffect stressMusic;
        SoundEffectInstance music;
        int musicChangeTimer;
        bool isStressMusic;

        InputState inputState;
        Keys[][] controllerMappings;

        bool isRunning;

        PlayerInput[] input = new PlayerInput[] {
            PlayerInput.Left, PlayerInput.Right, PlayerInput.Up, PlayerInput.Down, PlayerInput.Swap, PlayerInput.Raise
        };
        Buttons[] buttons = new Buttons[] {
            Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadUp, Buttons.DPadDown, Buttons.A, Buttons.RightShoulder
        };
        Keys[][] keyMappings = new Keys[][] {
            new Keys[]{Keys.A, Keys.D, Keys.W, Keys.S, Keys.LeftControl, Keys.LeftShift},
            new Keys[]{Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.RightControl, Keys.RightShift},
        };

        public TetattGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Components.Add(new Background(this));

            playFields = new PlayField[4];
            for (int i = 0; i < 4; i++)
            {
                var dp = new DrawablePlayField(this, new Vector2(96 + 288 * i, 248));
                playFields[i] = dp.PlayField;
                playFields[i].PerformedCombo += playField_PerformedCombo;
                playFields[i].PerformedChain += playField_PerformedChain;
                playFields[i].Popped += playField_Popped;
                playFields[i].Died += playField_Died;
                Components.Add(dp);
            }

            inputState = new InputState();
            controllerMappings = new Keys[][] { keyMappings[0], keyMappings[1], null, null };
            isRunning = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            popEffect = new SoundEffect[4];
            for (int i = 0; i < popEffect.Length; i++)
            {
                popEffect[i] = Content.Load<SoundEffect>("pop" + (i + 1));
            }
            chainEffect = Content.Load<SoundEffect>("chain");
            fanfare1Effect = Content.Load<SoundEffect>("fanfare1");
            fanfare2Effect = Content.Load<SoundEffect>("fanfare2");

            normalMusic = Content.Load<SoundEffect>("normal_music");
            stressMusic = Content.Load<SoundEffect>("stress_music");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateInput();

            bool anyStress = playFields.Any(p => p.GetHeight() >= PlayField.stressHeight);
            if (anyStress != isStressMusic && --musicChangeTimer <= 0)
            {
                music.Dispose();
                music = (anyStress ? stressMusic : normalMusic).CreateInstance();
                music.IsLooped = true;
                music.Play();
                isStressMusic = anyStress;
                musicChangeTimer = 20;
            }

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            inputState.Update();

            // Allows the game to exit
            if (inputState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (isRunning)
            {
                for (int i = 0; i < playFields.Length; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (inputState.IsButtonDownThisFrame(i, buttons[j]) ||
                            (controllerMappings[i] != null && inputState.IsKeyDownThisFrame(controllerMappings[i][j])))
                        {
                            playFields[i].Input((PlayerInput)j);
                        }
                    }
                    if (inputState.IsButtonDown(i, buttons[5]) ||
                        (controllerMappings[i] != null && inputState.IsKeyDown(controllerMappings[i][5])))
                    {
                        playFields[i].Input(input[5]);
                    }
                }
            }
            else
            {
                if (inputState.IsKeyDown(Keys.Enter))
                {
                    isRunning = true;
                    foreach (var p in playFields)
                    {
                        p.Reset();
                        p.Start();
                    }
                    music = normalMusic.CreateInstance();
                    music.IsLooped = true;
                    music.Play();
                }

                for (int i = 0; i < playFields.Length; i++)
                {
                    if (inputState.IsButtonDownThisFrame(i, buttons[2]) ||
                        (controllerMappings[i] != null && inputState.IsKeyDownThisFrame(controllerMappings[i][2])))
                    {
                        playFields[i].Level = Math.Min(playFields[i].Level + 1, 9);
                    }
                    if (inputState.IsButtonDownThisFrame(i, buttons[3]) ||
                        (controllerMappings[i] != null && inputState.IsKeyDownThisFrame(controllerMappings[i][3])))
                    {
                        playFields[i].Level = Math.Max(playFields[i].Level - 1, 0);
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            base.Draw(gameTime);
        }

        private void playField_PerformedCombo(PlayField sender, Pos pos, bool isChain, int count)
        {
            if (isChain)
            {
                chainEffect.Play();
            }
        }

        private void playField_PerformedChain(PlayField sender, Chain chain)
        {
            foreach (GarbageInfo info in chain.garbage)
            {
                GetGarbageTarget(sender).AddGarbage(info.size, info.type);
            }
            if (chain.length > 1)
            {
                GetGarbageTarget(sender).AddGarbage(chain.length - 1, GarbageType.Chain);
            }

            if (chain.length == 4)
            {
                fanfare1Effect.Play();
            }
            else if (chain.length > 4)
            {
                fanfare2Effect.Play();
            }
        }

        private void playField_Popped(PlayField sender, Pos pos, bool isGarabge, Chain chain)
        {
            SoundEffect effect = popEffect[Math.Min(chain.length, 4) - 1];
            effect.Play(1, chain.popCount / 10.0f, 0);

            if (chain.popCount < 10)
            {
                chain.popCount++;
            }
        }

        private void playField_Died(PlayField sender)
        {
            int alive = playFields.Count(p => p.State == PlayFieldState.Play);
            if (alive <= 1)
            {
                isRunning = false;
                GetGarbageTarget(sender).Stop();
                music.Stop();
            }
        }

        private PlayField GetGarbageTarget(PlayField playField)
        {
            int index = Array.IndexOf(playFields, playField);
            for (int i = 1; i < playFields.Length; i++)
            {
                var p = playFields[(index + i) % playFields.Length];
                if (p.State == PlayFieldState.Play)
                    return p;
            }
            return playField; // Should only happen when we are about to win...
        }
    }
}
