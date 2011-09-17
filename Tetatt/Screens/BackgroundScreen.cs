
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tetatt.Screens
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : Screen
    {
        VertexBuffer vertexBuffer;
        BasicEffect effect;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen(ScreenManager manager)
            : base(manager)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this screen. This will create the vertex buffer
        /// and effects used for this screen.
        /// </summary>
        public override void LoadContent()
        {
            // Triangle strip with four triangles that cover the screen
            // with a gradient in two parts
            VertexPositionColor[] data = new VertexPositionColor[6] {
                new VertexPositionColor(
                    new Vector3(0, 0, 0),
                    new Color(0.5f, 0.5f, 0.5f)),
                new VertexPositionColor(
                    new Vector3(1280, 0, 0),
                    new Color(0.5f, 0.5f, 0.5f)),
                new VertexPositionColor(
                    new Vector3(0, 648, 0),
                    new Color(0.0f, 0.0f, 0.0f)),
                new VertexPositionColor(
                    new Vector3(1280, 640, 0),
                    new Color(0.0f, 0.0f, 0.0f)),
                new VertexPositionColor(
                    new Vector3(0, 720, 0),
                    new Color(0.24f, 0.24f, 0.24f)),
                new VertexPositionColor(
                    new Vector3(1280, 720, 0),
                    new Color(0.24f, 0.24f, 0.24f)),
            };

            vertexBuffer = new VertexBuffer(ScreenManager.GraphicsDevice, typeof(VertexPositionColor), data.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(data);

            effect = new BasicEffect(ScreenManager.GraphicsDevice);
            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, -1, 1);
            effect.VertexColorEnabled = true;

            base.LoadContent();
        }

        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                ScreenManager.GraphicsDevice.DrawPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    4
                );
            }

            base.Draw(gameTime);
        }
    }
}
