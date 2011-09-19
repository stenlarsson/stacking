using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;

namespace Tetatt.Screens
{
    class SplashScreen : Screen
    {
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

        const string fontLicense = @"Bitstream Vera Fonts Copyright
------------------------------

Copyright (c) 2003 by Bitstream, Inc. All Rights Reserved. Bitstream Vera is
a trademark of Bitstream, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy
of the fonts accompanying this license (""Fonts"") and associated
documentation files (the ""Font Software""), to reproduce and distribute the
Font Software, including without limitation the rights to use, copy, merge,
publish, distribute, and/or sell copies of the Font Software, and to permit
persons to whom the Font Software is furnished to do so, subject to the
following conditions:

The above copyright and trademark notices and this permission notice
shall be included in all copies of one or more of the Font Software
typefaces.
";

        int preloadIndex;

        public SplashScreen(ScreenManager manager)
            : base(manager)
        {
            preloadIndex = 0;
            TransitionOffTime = TimeSpan.FromSeconds(1);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            var game = ScreenManager.Game;
            var content = game.Content;
            var components = game.Components;

            if (preloadIndex == preloadAssets.Length)
            {
                components.Add(new MessageDisplayComponent(game));
                components.Add(new GamerServicesComponent(game));
                components.Add(new AudioComponent(game));
                components.Add(new RankingsStorage(game));
                ExitScreen();
                ScreenManager.AddScreen(new BackgroundScreen(ScreenManager), null);
                ScreenManager.AddScreen(new MainMenuScreen(ScreenManager), null);
            }
            else if (preloadIndex < preloadAssets.Length)
            {
                content.Load<object>(preloadAssets[preloadIndex]);
            }

            preloadIndex++;

            // Don't try to catch up after long frame (due to loading)
            ScreenManager.Game.ResetElapsedTime();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            spriteBatch.Begin();

            Vector2 measure = ScreenManager.Font.MeasureString(fontLicense);

            spriteBatch.DrawString(
                ScreenManager.Font,
                fontLicense,
                new Vector2(viewport.Width, viewport.Height) / 2,
                Color.White * TransitionAlpha,
                0,
                measure / 2,
                0.5f,
                SpriteEffects.None,
                0);

            spriteBatch.End();
        }
    }
}
