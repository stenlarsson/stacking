using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Tetatt.GamePlay;

namespace Tetatt.Graphics
{
    public class EffCombo : DrawableGameComponent
    {
        private int tile;
        private int duration;
        private Vector2 pos;
        private SpriteBatch spriteBatch;

        public EffCombo(Game game, Vector2 pos, bool isChain, int count)
            : base(game)
        {
            this.pos = pos;
            tile = (isChain ? 52 : 38) + count;
            // TODO difficulty
            //g_game->GetLevelData()->effComboDuration
            duration = 80;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            duration--;
            if (duration <= 0)
            {
                Game.Components.Remove(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            pos.Y -= duration >> 5;
            spriteBatch.Begin();
            spriteBatch.Draw(
                DrawablePlayField.blocksTileSet.Texture,
                pos,
                DrawablePlayField.blocksTileSet.SourceRectangle(tile),
                Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}