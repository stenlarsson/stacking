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

        DrawablePlayField playField1;
        DrawablePlayField playField2;

        SoundEffect normalMusic;
        SoundEffect stressMusic;
        SoundEffectInstance music;
        int musicChangeTimer;
        bool isStressMusic;

        InputState inputState;

        bool isRunning;

        public TetattGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Components.Add(new Background(this));

            playField1 = new DrawablePlayField(this, new Vector2(96, 248));
            playField1.PlayField.PerformedCombo += playField_PerformedCombo;
            playField1.PlayField.PerformedChain += playField_PerformedChain;
            playField1.PlayField.Popped += playField_Popped;
            playField1.PlayField.Died += playField_Died;
            Components.Add(playField1);

            playField2 = new DrawablePlayField(this, new Vector2(384, 248));
            playField2.PlayField.PerformedCombo += playField_PerformedCombo;
            playField2.PlayField.PerformedChain += playField_PerformedChain;
            playField2.PlayField.Popped += playField_Popped;
            playField2.PlayField.Died += playField_Died;
            Components.Add(playField2);

            inputState = new InputState();
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

            if (playField1.PlayField.GetHeight() >= PlayField.stressHeight ||
                playField2.PlayField.GetHeight() >= PlayField.stressHeight)
            {
                if (!isStressMusic && musicChangeTimer <= 0)
                {
                    music.Dispose();
                    music = stressMusic.CreateInstance();
                    music.IsLooped = true;
                    music.Play();
                    isStressMusic = true;
                }
                musicChangeTimer = 20;
            }
            else
            {
                if (isStressMusic && --musicChangeTimer <= 0)
                {
                    music.Dispose();
                    music = normalMusic.CreateInstance();
                    music.IsLooped = true;
                    music.Play();
                    isStressMusic = false;
                }
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
                if (inputState.IsButtonDownThisFrame(0, Buttons.DPadLeft) ||
                    inputState.IsKeyDownThisFrame(Keys.A))
                {
                    playField1.PlayField.MoveLeft();
                }

                if (inputState.IsButtonDownThisFrame(0, Buttons.DPadRight) ||
                    inputState.IsKeyDownThisFrame(Keys.D))
                {
                    playField1.PlayField.MoveRight();
                }

                if (inputState.IsButtonDownThisFrame(0, Buttons.DPadUp) ||
                    inputState.IsKeyDownThisFrame(Keys.W))
                {
                    playField1.PlayField.MoveUp();
                }

                if (inputState.IsButtonDownThisFrame(0, Buttons.DPadDown) ||
                    inputState.IsKeyDownThisFrame(Keys.S))
                {
                    playField1.PlayField.MoveDown();
                }

                if (inputState.IsButtonDownThisFrame(0, Buttons.A) ||
                    inputState.IsKeyDownThisFrame(Keys.LeftControl))
                {
                    playField1.PlayField.Swap();
                }

                if (inputState.IsButtonDown(0, Buttons.RightShoulder) ||
                    inputState.IsKeyDown(Keys.LeftShift))
                {
                    playField1.PlayField.Raise();
                }


                if (inputState.IsButtonDownThisFrame(1, Buttons.DPadLeft) ||
                    inputState.IsKeyDownThisFrame(Keys.Left))
                {
                    playField2.PlayField.MoveLeft();
                }

                if (inputState.IsButtonDownThisFrame(1, Buttons.DPadRight) ||
                    inputState.IsKeyDownThisFrame(Keys.Right))
                {
                    playField2.PlayField.MoveRight();
                }

                if (inputState.IsButtonDownThisFrame(1, Buttons.DPadUp) ||
                    inputState.IsKeyDownThisFrame(Keys.Up))
                {
                    playField2.PlayField.MoveUp();
                }

                if (inputState.IsButtonDownThisFrame(1, Buttons.DPadDown) ||
                    inputState.IsKeyDownThisFrame(Keys.Down))
                {
                    playField2.PlayField.MoveDown();
                }

                if (inputState.IsButtonDownThisFrame(1, Buttons.A) ||
                    inputState.IsKeyDownThisFrame(Keys.RightControl))
                {
                    playField2.PlayField.Swap();
                }

                if (inputState.IsButtonDown(1, Buttons.RightShoulder) ||
                    inputState.IsKeyDown(Keys.RightShift))
                {
                    playField2.PlayField.Raise();
                }
            }
            else
            {
                if (inputState.IsKeyDown(Keys.Enter))
                {
                    isRunning = true;
                    playField1.PlayField.Reset();
                    playField2.PlayField.Reset();
                    playField1.PlayField.Start();
                    playField2.PlayField.Start();
                    music = normalMusic.CreateInstance();
                    music.IsLooped = true;
                    music.Play();
                }

                if (inputState.IsButtonDownThisFrame(0, Buttons.DPadUp) ||
                    inputState.IsKeyDownThisFrame(Keys.W))
                {
                    playField1.PlayField.Level = Math.Min(playField1.PlayField.Level + 1, 9);
                }

                if (inputState.IsButtonDownThisFrame(0, Buttons.DPadDown) ||
                    inputState.IsKeyDownThisFrame(Keys.S))
                {
                    playField1.PlayField.Level = Math.Max(playField1.PlayField.Level - 1, 0);
                }

                if (inputState.IsButtonDownThisFrame(1, Buttons.DPadUp) ||
                    inputState.IsKeyDownThisFrame(Keys.Up))
                {
                    playField2.PlayField.Level = Math.Min(playField2.PlayField.Level + 1, 9);
                }

                if (inputState.IsButtonDownThisFrame(1, Buttons.DPadDown) ||
                    inputState.IsKeyDownThisFrame(Keys.Down))
                {
                    playField2.PlayField.Level = Math.Max(playField2.PlayField.Level - 1, 0);
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
                OtherPlayField(sender).AddGarbage(info.size, info.type);
            }
            if (chain.length > 1)
            {
                OtherPlayField(sender).AddGarbage(chain.length - 1, GarbageType.Chain);
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
            isRunning = false;
            OtherPlayField(sender).Stop();
            music.Stop();
        }

        private PlayField OtherPlayField(PlayField playField)
        {
            if (playField == playField1.PlayField)
            {
                return playField2.PlayField;
            }
            else
            {
                return playField1.PlayField;
            }
        }
    }
}
