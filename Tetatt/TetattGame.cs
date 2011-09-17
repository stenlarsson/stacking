using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetatt.Screens;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using System.Reflection;
using Tetatt.Networking;
using Tetatt.Graphics;
using Tetatt.ArtificialIntelligence;

namespace Tetatt
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TetattGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "blank",
            "blocks",
            "cat",
            "chat_able",
            "chat_mute",
            "chat_ready",
            "chat_talking",
            "fanfare1",
            "fanfare2",
            "gradient",
            "ingame_font",
            "logo",
            "marker",
            "normal_music",
            "playfield",
            "pop1",
            "pop2",
            "pop3",
            "pop4",
            "normal_music",
            "stress_music",
        };

        public TetattGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);
            Components.Add(new MessageDisplayComponent(this));
            Components.Add(new GamerServicesComponent(this));
            Components.Add(new AudioComponent(this));
            Components.Add(new RankingsStorage(this));

            // load the initial screens
            screenManager.AddScreen(new BackgroundScreen(screenManager), null);
            screenManager.AddScreen(new MainMenuScreen(screenManager), null);

            // Listen for invite notification events.
            NetworkSession.InviteAccepted += (sender, e)
                => NetworkSessionComponent.InviteAccepted(screenManager, e);

            // To test the trial mode behavior while developing your game,
            // uncomment this line:

            // Guide.SimulateTrialMode = true;
        }

        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}
