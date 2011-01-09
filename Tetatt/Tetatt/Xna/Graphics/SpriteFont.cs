using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteFont
    {
        internal Graphics.Texture2D texture;
        internal IDictionary<char, Rectangle> chars;

        public SpriteFont(Graphics.Texture2D texture, IDictionary<char, Rectangle> chars) {
            this.texture = texture;
            this.chars = chars;
            Spacing = 0.0f;
        }

        public float Spacing { get; set; }
        public Vector2 MeasureString(string text)
        {
            int x = 0, y = 0;
            foreach (char c in text)
            {
                x += chars[c].Width;
                y = Math.Max(y, chars[c].Height);
            }
            return new Vector2(x + (text.Length - 1)*Spacing, y);
        }
    }
}

