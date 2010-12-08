using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
        Texture2D background;
        SoundEffect[] popEffect;
        SoundEffect chainEffect;
        SoundEffect fanfare1Effect;
        SoundEffect fanfare2Effect;

        DrawablePlayField playField;
        Vector2 playFieldOffset;

        GamePadState oldGamePadState;
        KeyboardState oldKeyboardState;

        public TetattGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            playField = new DrawablePlayField();
            playField.PerformedCombo += playField_PerformedCombo;
            playField.PerformedChain += playField_PerformedChain;
            playField.Popped += playField_Popped;

            playFieldOffset = new Vector2(96, 248);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
            oldKeyboardState = Keyboard.GetState();
            playField.Start();

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

            background = this.Content.Load<Texture2D>("background");
            DrawablePlayField.background = this.Content.Load<Texture2D>("playfield");
            DrawablePlayField.marker = this.Content.Load<Texture2D>("marker");
            DrawablePlayField.blocksTileSet = new TileSet(
                this.Content.Load<Texture2D>("blocks"), DrawablePlayField.blockSize);

            popEffect = new SoundEffect[4];
            for (int i = 0; i < popEffect.Length; i++) {
                popEffect[i] = Content.Load<SoundEffect>("pop" + (i+1));
            }
            chainEffect = Content.Load<SoundEffect>("chain");
            fanfare1Effect = Content.Load<SoundEffect>("fanfare1");
            fanfare2Effect = Content.Load<SoundEffect>("fanfare2");
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

            playField.Update();

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (gamePadState.Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (gamePadState.IsButtonDown(Buttons.DPadLeft) &&
                !oldGamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.A) &&
                !oldKeyboardState.IsKeyDown(Keys.A))
                playField.KeyInput(InputType.Left);

            if (gamePadState.IsButtonDown(Buttons.DPadRight) &&
                !oldGamePadState.IsButtonDown(Buttons.DPadRight) ||
                keyboardState.IsKeyDown(Keys.D) &&
                !oldKeyboardState.IsKeyDown(Keys.D))
                playField.KeyInput(InputType.Right);

            if (gamePadState.IsButtonDown(Buttons.DPadUp) &&
                !oldGamePadState.IsButtonDown(Buttons.DPadUp) ||
                keyboardState.IsKeyDown(Keys.W) &&
                !oldKeyboardState.IsKeyDown(Keys.W))
                playField.KeyInput(InputType.Up);

            if (gamePadState.IsButtonDown(Buttons.DPadDown) &&
                !oldGamePadState.IsButtonDown(Buttons.DPadDown) ||
                keyboardState.IsKeyDown(Keys.S) &&
                !oldKeyboardState.IsKeyDown(Keys.S))
                playField.KeyInput(InputType.Down);

            if (gamePadState.IsButtonDown(Buttons.A) &&
                !oldGamePadState.IsButtonDown(Buttons.A) ||
                keyboardState.IsKeyDown(Keys.LeftControl) &&
                !oldKeyboardState.IsKeyDown(Keys.LeftControl))
                playField.KeyInput(InputType.Swap);

            if (gamePadState.IsButtonDown(Buttons.RightShoulder) ||
                keyboardState.IsKeyDown(Keys.LeftShift))
                playField.KeyInput(InputType.Raise);

            oldGamePadState = gamePadState;
            oldKeyboardState = keyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            playField.Draw(spriteBatch, playFieldOffset);
            base.Draw(gameTime);
        }

        private void playField_PerformedCombo(object sender, ComboEventArgs ce)
        {
            EffCombo eff = new EffCombo(
                this,
                playField.PosToVector(ce.pos) + playFieldOffset,
                ce.isChain,
                ce.count);
            Components.Add(eff);

            if (ce.isChain)
            {
                chainEffect.Play();
            }
        }

        private void playField_PerformedChain(object sender, ChainEventArgs ce)
        {
            if (ce.chain.length < 4)
            {
                return;
            }
            else if (ce.chain.length == 4)
            {
                fanfare1Effect.Play();
            }
            else
            {
                fanfare2Effect.Play();
            }
        }

        private void playField_Popped(object sender, PoppedEventArgs pe)
        {
            SoundEffect effect = popEffect[Math.Min(pe.chain.length, 4)-1];
            effect.Play(1, pe.chain.popCount / 10.0f, 0);

            if (pe.chain.popCount < 10)
            {
                pe.chain.popCount++;
            }
        }
    }
}
