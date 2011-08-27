
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetatt.ArtificialIntelligence;
using Tetatt.GamePlay;
using Tetatt.Graphics;
using System.Collections.Generic;

namespace Tetatt.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class VersusAIScreen : GameScreen
    {
        public const int NumStages = 10;

        public int Level { get; private set; }
        public int Stage { get; private set; }
        public bool GameOver { get; private set; }
        public int[] Times { get; private set; }

        public static Vector2[] Offsets = new Vector2[] {
            new Vector2(384, 248),
            new Vector2(672, 248),
        };

        float pauseAlpha;


        DrawablePlayField playerPlayField;
        DrawablePlayField aiPlayField;

        AIPlayer aiPlayer;

        AudioComponent audioComponent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VersusAIScreen(int level, ScreenManager screenManager)
        {
            Level = level;
            Stage = 0;
            GameOver = false;
            Times = new int[NumStages];

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            playerPlayField = new DrawablePlayField(Player.DefaultLevel);
            playerPlayField.PerformedChain += PerformedChain;
            playerPlayField.State = PlayFieldState.Dead;
            aiPlayField = new DrawablePlayField(Player.DefaultLevel);
            aiPlayField.PerformedChain += PerformedChain;
            aiPlayField.State = PlayFieldState.Dead;

            aiPlayer = new AIPlayer(aiPlayField);

            // Cannot use ScreenManager here yet because we're not yet added, therefore
            // it must be passed as a paramter so that we can get the AudioComponent.
            audioComponent = (AudioComponent)screenManager.Game.Services.GetService(
                typeof(AudioComponent));
            audioComponent.AddPlayField(playerPlayField);
            audioComponent.AddPlayField(aiPlayField);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // Load graphics
            DrawablePlayField.background = content.Load<Texture2D>("playfield");
            DrawablePlayField.marker = content.Load<Texture2D>("marker");
            DrawablePlayField.blocksTileSet = new TileSet(
                content.Load<Texture2D>("blocks"), DrawablePlayField.BlockSize);
            DrawablePlayField.font = content.Load<SpriteFont>("ingame_font");
            DrawablePlayField.font.Spacing = -5;
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

            if (IsExiting)
            {
                return;
            }


            // Show stage screen if we're not playing and not already showing it
            if (playerPlayField.State == PlayFieldState.Dead &&
                aiPlayField.State == PlayFieldState.Dead &&
                !coveredByOtherScreen)
            {
                ScreenManager.AddScreen(new StageScreen(this), ControllingPlayer);
            }



            CheckEndOfGame();

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);


            // Update playfields if not paused
            if (IsActive)
            {
                playerPlayField.Update();
                aiPlayField.Update();
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

            PlayerIndex playerIndex = ControllingPlayer.Value;

            // Check if player paused the game
            if (input.IsPauseGame(playerIndex, out playerIndex))
            {
                ShowPauseScreen(playerIndex);
                return;
            }

            int playerInt = (int)playerIndex;

            GamePadState gamePadState = input.CurrentGamePadStates[playerInt];

            // The game pauses if the user unplugs the active gamepad. This requires
            // us to keep track of whether a gamepad was ever plugged in, because we
            // don't want to pause on PC if they are playing with a keyboard and have
            // no gamepad at all!
            if (!gamePadState.IsConnected && input.GamePadWasConnected[playerInt])
            {
                ShowPauseScreen(playerIndex);
                return;
            }

            playerPlayField.Input(input.GetPlayerInput(playerIndex));
            aiPlayField.Input(aiPlayer.GetInput());
        }

        /// <summary>
        /// Screen-specific update to gamer rich presence.
        /// </summary>
        public override void UpdatePresence()
        {
            if (!IsExiting)
            {
                SignedInGamer gamer = Gamer.SignedInGamers[ControllingPlayer.Value];
                if (gamer != null && gamer.IsSignedInToLive)
                {
                    gamer.Presence.PresenceMode = GamerPresenceMode.VersusComputer;
                }
            }
        }
        
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            playerPlayField.Offset = Offsets[0];
            playerPlayField.Draw(ScreenManager, gameTime, TransitionAlpha);
            aiPlayField.Offset = Offsets[1];
            aiPlayField.Draw(ScreenManager, gameTime, TransitionAlpha);

            base.Draw(gameTime);

            // If the game is covered by pause screen, fade it out to black.
            if (pauseAlpha > 0)
            {
                ScreenManager.FadeBackBufferToBlack(pauseAlpha / 2);
            }
        }

        /// <summary>
        /// Show message box asking if player wants to exit
        /// </summary>
        public void ShowPauseScreen(PlayerIndex playerIndex)
        {
            // Display a message box to confirm the user really wants to leave.
            string message;

            message = Resources.ConfirmEndSession;

            MessageBoxScreen confirmMessageBox = new MessageBoxScreen(message);

            // Hook the messge box ok event to actually leave the session.
            confirmMessageBox.Accepted += delegate
            {
                ScreenManager.ReturnToMainMenu();
            };

            ScreenManager.AddScreen(confirmMessageBox, playerIndex);
        }

        /// <summary>
        /// Called when a chain is completed
        /// </summary>
        private void PerformedChain(PlayField sender, Chain chain)
        {
            PlayField receiver = (sender == playerPlayField) ? aiPlayField : playerPlayField;

            foreach (GarbageInfo info in chain.garbage)
            {
                receiver.AddGarbage(info.size, info.type);
            }
            if (chain.length > 1)
            {
                receiver.AddGarbage(chain.length - 1, GarbageType.Chain);
            }
        }

        /// <summary>
        /// Called by Stage screen to start game
        /// </summary>
        public void Start()
        {
            aiPlayer.SetDifficulty(Level, Stage);

            int seed = unchecked((int)DateTime.Now.Ticks);
            playerPlayField.Reset();
            playerPlayField.Start(seed);
            aiPlayField.Reset();
            aiPlayField.Start(seed);

            audioComponent.GameStarted();
        }

        /// <summary>
        /// Check end of game. Should only be called if we are the host.
        /// </summary>
        private void CheckEndOfGame()
        {
            // If player died first
            if (playerPlayField.State == PlayFieldState.Die &&
                aiPlayField.State == PlayFieldState.Play)
            {
                aiPlayField.State = PlayFieldState.Dead;
                GameOver = true;
                audioComponent.GameEnded();
            }
            
            // If ai died first
            if (aiPlayField.State == PlayFieldState.Die &&
                playerPlayField.State == PlayFieldState.Play)
            {
                playerPlayField.State = PlayFieldState.Dead;
                Times[Stage] = playerPlayField.Time;
                Stage += 1;

                if (Stage == NumStages)
                {
                    GameOver = true;
                }

                audioComponent.GameEnded();
            }
        }

        /// <summary>
        /// Called when screen is exiting
        /// </summary>
        public override void ExitScreen()
        {
            audioComponent.Reset();

            base.ExitScreen();
        }
    }
}
