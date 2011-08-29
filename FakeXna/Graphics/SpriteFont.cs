using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteFont
    {
        internal Graphics.Texture2D texture;
        IDictionary<char, Rectangle> chars;
        Rectangle backupRect;

        public SpriteFont(Graphics.Texture2D texture, IDictionary<char, Rectangle> chars)
        {
            this.texture = texture;
            this.chars = chars;
            this.Spacing = 0.0f;
            this.LineSpacing = chars.First().Value.Height;
            this.backupRect = new Rectangle(0,0,0,0);
        }

        public ReadOnlyCollection<char> Characters {
            get { return new ReadOnlyCollection<char>(chars.Keys.ToList()); }
        }
        public char? DefaultCharacter { get; set; }
        public float Spacing { get; set; }
        public int LineSpacing { get; set; }
        public Vector2 MeasureString(string text)
        {
            Vector2 size = Vector2.Zero;
            foreach (string line in Lines(text))
            {
                int length = line.Sum((c) => SourceRectangle(c).Width);
                size.X = Math.Max(size.X, length + Spacing * (line.Length - 1));
                size.Y += LineSpacing;
            }
            return size;
        }

        internal delegate void _DrawAction(Vector2 position, Rectangle source);
        internal void EachChar(string text, _DrawAction callback)
        {
            Vector2 pos = Vector2.Zero;
            foreach (string line in Lines(text))
            {
                pos.X = 0;
                foreach (char c in line)
                {
                    Rectangle rect = SourceRectangle(c);
                    callback(pos, rect);
                    pos.X += rect.Width + Spacing;
                }
                pos.Y += LineSpacing;
            }
        }

        Rectangle SourceRectangle(char c)
        {
            Rectangle rect = backupRect;
            if (!chars.TryGetValue(c, out rect) && DefaultCharacter == null)
                throw new ArgumentException(string.Format("Unsupported character '{0}'", c));
            return rect;
        }

        IEnumerable<string> Lines(string text)
        {
            int start = 0, end = 0;
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                case '\r':
                    continue;
                case '\n':
                    yield return text.Substring(start, end-start);
                    start = end = i+1;
                    break;
                default:
                    end++;
                    break;
                }
            }
            if (text.Length > 0)
                yield return text.Substring(start, end-start);
        }

        internal static SpriteFont _FromTextureAndXmlStream(Texture2D texture, Stream stream)
        {
            return new SpriteFont(texture,
                XDocument.Load(stream).Root.Elements("character").ToDictionary(
                    e => (char)(int)e.Attribute("key"),
                    e => new Rectangle(
                         (int)e.Element("x"),
                         (int)e.Element("y"),
                         (int)e.Element("width"),
                         (int)e.Element("height"))
                    )
                );
        }
    }
}

