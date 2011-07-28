using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetatt.GamePlay;
using Tetatt.Screens;

namespace Tetatt.Graphics
{
    public class EffPop : DrawableGameComponent
    {
        private static AnimFrame[] frames =
        {
            new AnimFrame(84, 3),
            new AnimFrame(85, 3),
            new AnimFrame(86, 3),
            new AnimFrame(87, 3),
            new AnimFrame(88, 3),
            new AnimFrame(89, 3),
            new AnimFrame(90, 3),
            new AnimFrame(91, 3),
            new AnimFrame(92, 3)
        };

        private int duration;
        private Vector2 pos;
        private int offset;
        private int mov;
        private SpriteBatch spriteBatch;
        private Anim anim;

        public EffPop(ScreenManager screenManager, Vector2 pos)
            : base(screenManager.Game)
        {
            this.spriteBatch = screenManager.SpriteBatch;
            this.pos = pos;
            duration = 9*3+1;
            offset = DrawablePlayField.BlockSize / 2;
            mov = 3;
            anim = new Anim(AnimType.Once, frames);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            anim.Update();

            if (mov > 0 && (duration & 1) == 1)
            {
                mov--;
            }
            offset += mov;

            duration--;
            if (duration <= 0)
            {
                Dispose();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                DrawablePlayField.blocksTileSet.Texture,
                pos + new Vector2(-offset, -offset),
                DrawablePlayField.blocksTileSet.SourceRectangle(anim.GetFrame()),
                Color.White);
            spriteBatch.Draw(
                DrawablePlayField.blocksTileSet.Texture,
                pos + new Vector2(offset, -offset),
                DrawablePlayField.blocksTileSet.SourceRectangle(anim.GetFrame()),
                Color.White);
            spriteBatch.Draw(
                DrawablePlayField.blocksTileSet.Texture,
                pos + new Vector2(-offset, offset),
                DrawablePlayField.blocksTileSet.SourceRectangle(anim.GetFrame()),
                Color.White);
            spriteBatch.Draw(
                DrawablePlayField.blocksTileSet.Texture,
                pos + new Vector2(offset, offset),
                DrawablePlayField.blocksTileSet.SourceRectangle(anim.GetFrame()),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}