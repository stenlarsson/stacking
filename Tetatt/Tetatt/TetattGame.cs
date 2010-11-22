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

        PlayField playField;

        GamePadState oldGamePadState;
        KeyboardState oldKeyboardState;

        public TetattGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            playField = new PlayField();
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

            PlayField.background = this.Content.Load<Texture2D>("playfield");
            PlayField.marker = this.Content.Load<Texture2D>("marker");
            PlayField.blocksTileSet = new TileSet(
                this.Content.Load<Texture2D>("blocks"), PlayField.blockSize);
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
            GraphicsDevice.Clear(Color.Black);
            playField.Draw(spriteBatch, 96, 248);
            base.Draw(gameTime);
        }
    }
}
