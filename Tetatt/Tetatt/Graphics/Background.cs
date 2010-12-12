using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetatt.Graphics
{
    class Background : DrawableGameComponent
    {
        VertexBuffer vertexBuffer;
        BasicEffect effect;

        public Background(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
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

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), data.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(data);

            effect = new BasicEffect(GraphicsDevice);
            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, -1, 1);
            effect.VertexColorEnabled = true;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    4
                );
            }

            base.Draw(gameTime);
        }
    }
}
