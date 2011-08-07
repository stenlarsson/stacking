using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteFont
    {
        internal Graphics.Texture2D texture;
        internal IDictionary<char, Rectangle> chars;
        char backup;

        public SpriteFont(Graphics.Texture2D texture, IDictionary<char, Rectangle> chars) {
            this.texture = texture;
            this.chars = chars;
            this.backup = chars.ContainsKey('?') ? '?' : chars.First().Key;
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
            foreach (char c in PrepareString(text))
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

        internal string PrepareString(string text)
        {
            StringBuilder result = new StringBuilder(text.Length);
            bool ignoreline = false;
            foreach (char c in text)
            {
                switch (c)
                {
                case '\n':
                    if (ignoreline)
                        break;
                    goto case '\0';
                case '\r':
                    ignoreline = true;
                    goto case '\0';
                case '\0':
                    result.Append('\n');
                    break;
                default:
                    if (chars.ContainsKey(c))
                        result.Append(c);
                    else
                        result.Append(backup);
                    break;
                }
            }
            return result.ToString();
        }

        internal static SpriteFont _FromTextureAndXmlStream(Texture2D texture, Stream stream)
        {
            return new SpriteFont(texture,
                XDocument.Load(stream).Root.Elements("character").ToDictionary(
                    e => (char)int.Parse(e.Attribute("key").Value),
                    e => new Rectangle(
                         int.Parse(e.Element("x").Value),
                         int.Parse(e.Element("y").Value),
                         int.Parse(e.Element("width").Value),
                         int.Parse(e.Element("height").Value))
                    )
                );
        }
    }
}

