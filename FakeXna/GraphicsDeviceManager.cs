using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    public class GraphicsDeviceManager
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        public GraphicsProfile GraphicsProfile { get; set; }

        public int PreferredBackBufferWidth { get; set; }
        public int PreferredBackBufferHeight { get; set; }

        public GraphicsDeviceManager(Game game)
        {
            GraphicsProfile = GraphicsProfile.HiDef;
            PreferredBackBufferWidth = 1280;
            PreferredBackBufferHeight = 720;

            game.Services.AddService(typeof(GraphicsDeviceManager), this);
        }

        public bool BeginDraw()
        {
            return true;
        }

        public void CreateDevice()
        {
            GraphicsAdapter graphicsAdapter = new GraphicsAdapter();
            PresentationParameters presentationParameters =
                new PresentationParameters {
                    BackBufferHeight = PreferredBackBufferHeight,
                    BackBufferWidth = PreferredBackBufferWidth
                };

            GraphicsDevice = new GraphicsDevice(graphicsAdapter, GraphicsProfile, presentationParameters);
        }

        public void EndDraw()
        {
            GraphicsDevice.Present();
        }
    }
}
