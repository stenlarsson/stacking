using System;
using System.Collections.Generic;
using System.Linq;

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
            LineSpacing = chars.First().Value.Height;
        }

        public float Spacing { get; set; }
        public int LineSpacing { get; set; }
        public Vector2 MeasureString(string text)
        {
            int x = 0;
            int y = chars.First().Value.Height;
            int line = 0;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    line = Math.Max(line, x);
                    x = 0;
                    y += LineSpacing;
                    continue;
                } 
                x += chars[c].Width + (int)Spacing;
            }
            return new Vector2(Math.Max(line, x), y);
        }
    }
}

