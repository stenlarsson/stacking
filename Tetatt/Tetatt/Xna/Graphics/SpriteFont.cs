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
        Graphics.Texture2D texture;
        IDictionary<char, Rectangle> chars;
        char backup;

        public SpriteFont(Graphics.Texture2D texture, IDictionary<char, Rectangle> chars)
        {
            this.texture = texture;
            this.chars = chars;
            this.backup = chars.ContainsKey('?') ? '?' : chars.First().Key;
            this.Spacing = 0.0f;
            this.LineSpacing = chars.First().Value.Height;
        }

        public float Spacing { get; set; }
        public int LineSpacing { get; set; }
        public Vector2 MeasureString(string text)
        {
            int x = 0;
            int y = chars.First().Value.Height;
            int line = 0;
            foreach (char c in CleanString(text))
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

        internal delegate void _DrawAction(Texture2D texture, Rectangle source, Vector2 position);
        internal void _Draw(string text, _DrawAction callback)
        {
            Vector2 pos = new Vector2(0, 0);
            foreach (char c in CleanString(text))
            {
                if (c == '\n')
                {
                    pos.X = 0;
                    pos.Y += LineSpacing;
                }
                else
                {
                    Rectangle rect = chars[c];
                    callback(texture, rect, pos);
                    pos.X += rect.Width + Spacing;
                }
            }
        }


        IEnumerable<char> CleanString(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                switch (c)
                {
                case '\r':
                    if (text[++i] != '\n')
                        i--;
                    goto case '\n';
                case '\n':
                    yield return '\n';
                    break;
                default:
                    if (chars.ContainsKey(c))
                        yield return c;
                    else
                        yield return backup;
                    break;
                }
            }
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

