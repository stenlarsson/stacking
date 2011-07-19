
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
using Microsoft.Xna.Framework.Media;

namespace Tetatt.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        ContentManager content;

        public const int blockSize = 32;

        public readonly List<Player> Players;

        // TODO accessor
        public static TileSet blocksTileSet;
        public static Texture2D background;
        public static Texture2D marker;
        public static SpriteFont font;

        SoundEffect[] popEffect;
        SoundEffect chainEffect;
        SoundEffect fanfare1Effect;
        SoundEffect fanfare2Effect;

        SoundEffect normalMusic;
        SoundEffect stressMusic;
        SoundEffectInstance music;
        int musicChangeTimer;
        bool isStressMusic;

        float pauseAlpha;

        private bool isRunning;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // Create list of players and add the player who started the game
            Players = new List<Player>();
            if (ControllingPlayer.HasValue)
                CreatePlayer(ControllingPlayer.Value);

            isRunning = false;
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Load graphics
            background = content.Load<Texture2D>("playfield");
            marker = content.Load<Texture2D>("marker");
            blocksTileSet = new TileSet(
                content.Load<Texture2D>("blocks"), blockSize);
            font = content.Load<SpriteFont>("ingame_font");
            font.Spacing = -5;

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
            
            // Once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (!isRunning && !IsExiting && !coveredByOtherScreen)
            {
                ScreenManager.AddScreen(new LobbyScreen(this), null);
            }

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            // Update playfields if not paused
            if (IsActive)
            {
                foreach (Player player in Players)
                {
                    player.PlayField.Update();
                }
            }

            // Switch between to stressful music if anyone reaches a certain height, or back if
            // everyone is below again. Use a delay to avoid changing too often.
            bool anyStress = Players.Any(p => p.PlayField.GetHeight() >= PlayField.stressHeight);
            if (anyStress != isStressMusic && --musicChangeTimer <= 0)
            {
                music.Dispose();
                music = (anyStress ? stressMusic : normalMusic).CreateInstance();
                music.IsLooped = true;
                music.Play();
                isStressMusic = anyStress;
                musicChangeTimer = 20;
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PlayerIndex playerIndex;

            // Check if anyone paused the game
            if (input.IsNewKeyPress(Keys.Escape, null, out playerIndex) ||
                input.IsNewButtonPress(Buttons.Back, null, out playerIndex))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), playerIndex);
                return;
            }

            // Check input for each player
            foreach (Player player in Players)
            {
                int playerInt = (int)player.Index;

                KeyboardState keyboardState = input.CurrentKeyboardStates[playerInt];
                GamePadState gamePadState = input.CurrentGamePadStates[playerInt];

                // The game pauses if the user unplugs the active gamepad. This requires
                // us to keep track of whether a gamepad was ever plugged in, because we
                // don't want to pause on PC if they are playing with a keyboard and have
                // no gamepad at all!
                if (!gamePadState.IsConnected && input.GamePadWasConnected[playerInt])
                {
                    ScreenManager.AddScreen(new PauseMenuScreen(), player.Index);
                    return;
                }

                PlayerInput? playerInput = input.GetPlayerInput(player.Index);
                if (playerInput.HasValue)
                {
                    player.PlayField.Input(playerInput.Value);
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            foreach (Player player in Players)
            {
                spriteBatch.Begin();

                // Draw frame and background
                spriteBatch.Draw(
                    background,
                    player.Offset - new Vector2(16, 16), // Adjust for the frame
                    Color.White);

                // Draw statistics
                string score = player.PlayField.Score.ToString();
                spriteBatch.DrawString(
                    font,
                    "Score",
                    new Vector2(0, -75) + player.Offset,
                    Color.White);
                spriteBatch.DrawString(
                    font,
                    score,
                    new Vector2(200 - font.MeasureString(score).X, -75) + player.Offset,
                    Color.White);

                string time = String.Format("{0}:{1:00}", player.PlayField.Time / 60, player.PlayField.Time % 60);
                spriteBatch.DrawString(
                    font,
                    "Time",
                    new Vector2(0, -45) + player.Offset,
                    Color.White);
                spriteBatch.DrawString(
                    font,
                    time,
                    new Vector2(200 - font.MeasureString(time).X, -45) + player.Offset,
                    Color.White);

                spriteBatch.End();

                // Setup sprite clipping using scissor test
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null,
                    new RasterizerState()
                    {
                        ScissorTestEnable = true
                    });
                spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                    (int)player.Offset.X,
                    (int)player.Offset.Y,
                    PlayField.width * blockSize,
                    PlayField.visibleHeight * blockSize);

                // Draw blocks
                player.PlayField.EachVisibleBlock((row, col, block) =>
                {
                    if (block != null)
                    {
                        int tile = block.Tile;
                        Vector2 pos = PosToVector(new Pos(row, col), player);
                        if(block.IsState(BlockState.Moving))
                            pos.X += (block.Right ? 1 : -1) * blockSize * block.StateDelay / 5;

                        spriteBatch.Draw(
                            blocksTileSet.Texture,
                            new Rectangle(
                                (int)pos.X,
                                (int)pos.Y,
                                blockSize,
                                blockSize),
                            blocksTileSet.SourceRectangle(tile),
                            (row == 0 || player.PlayField.State == PlayFieldState.Dead) ? Color.DarkGray : Color.White);
                    }
                });

                spriteBatch.End();

                spriteBatch.Begin();
                if (player.PlayField.State == PlayFieldState.Play || player.PlayField.State == PlayFieldState.Start)
                {
                    // Draw marker
                    spriteBatch.Draw(
                        marker,
                        PosToVector(player.PlayField.markerPos, player) - new Vector2(4, 5),
                        Color.White);
                }
                spriteBatch.End();
            }

            base.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        public Vector2 PosToVector(Pos pos, Player player)
        {
            return new Vector2(
                pos.Col * blockSize + player.Offset.X,
                (PlayField.visibleHeight - pos.Row) * blockSize + (int)(player.PlayField.scrollOffset * blockSize) + player.Offset.Y);
        }


        /// <summary>
        /// Create Player and PlayField and add to Players
        /// </summary>
        /// <param name="playerIndex">Controller of new player</param>
        public void CreatePlayer(PlayerIndex playerIndex)
        {
            Player player = new Player(playerIndex);
            player.Offset = Player.Offsets[Players.Count];

            player.PlayField = new PlayField(player.StartLevel);
            player.PlayField.PerformedCombo += playField_PerformedCombo;
            player.PlayField.PerformedChain += playField_PerformedChain;
            player.PlayField.Popped += playField_Popped;
            player.PlayField.Died += playField_Died;

            Players.Add(player);
        }

        public void StartGame()
        {
            foreach (var p in Players)
            {
                p.PlayField.Reset();
                p.PlayField.Level = p.StartLevel;
                p.PlayField.Start();
            }
            music = normalMusic.CreateInstance();
            music.IsLooped = true;
            music.Play();
            isRunning = true;
        }

        public void StopGame()
        {
            foreach (var p in Players)
            {
                p.PlayField.Stop();
            }
            music.Stop();
            isRunning = false;
        }
        
        private void playField_PerformedCombo(PlayField sender, Pos pos, bool isChain, int count)
        {
            Player player = GetPlayer(sender);
            ScreenManager.Game.Components.Add(
                new EffCombo(ScreenManager, PosToVector(pos, player),
                    isChain, count,
                    sender.GetLevelData().effComboDuration));

            if (isChain)
            {
                chainEffect.Play();
            }
        }

        private void playField_PerformedChain(PlayField sender, Chain chain)
        {
            Player player = GetPlayer(sender);
            foreach (GarbageInfo info in chain.garbage)
            {
                GetGarbageTarget(player).PlayField.AddGarbage(info.size, info.type);
            }
            if (chain.length > 1)
            {
                GetGarbageTarget(player).PlayField.AddGarbage(chain.length - 1, GarbageType.Chain);
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
            Player player = GetPlayer(sender);
            ScreenManager.Game.Components.Add(
                new EffPop(ScreenManager, PosToVector(pos, player)));

            SoundEffect effect = popEffect[Math.Min(chain.length, 4) - 1];
            effect.Play(1, chain.popCount / 10.0f, 0);

            if (chain.popCount < 10)
            {
                chain.popCount++;
            }
        }

        private void playField_Died(PlayField sender)
        {
            Player player = GetPlayer(sender);
            var alive = Players.FindAll(p => p.PlayField.State == PlayFieldState.Play);
            if (alive.Count <= 1)
            {
                StopGame();
                if (alive.Count == 1)
                    alive[0].Wins++;
            }
        }

        private Player GetPlayer(PlayField playField)
        {
            return Players.First(p => p.PlayField == playField);
        }

        private Player GetGarbageTarget(Player player)
        {
            int index = Players.IndexOf(player);
            for (int i = 1; i < Players.Count; i++)
            {
                var p = Players[(index + i) % Players.Count];
                if (p.PlayField.State == PlayFieldState.Play)
                    return p;
            }
            return player; // Should only happen when we are about to win...
        }
    }
}
