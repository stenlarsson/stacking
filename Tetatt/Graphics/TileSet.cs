using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tetatt.Graphics
{
    /// <summary>
    /// A tile set contains a number of equally sized images (tiles) laid ut
    /// in two dimenstions, but indexed in only one dimension. This way you can have 
    /// </summary>
    public class TileSet
    {
        List<Rectangle> rectangles;

        public TileSet(Texture2D texture, int tileSize)
        {
            Texture = texture;
            TileSize = tileSize;

            rectangles = new List<Rectangle>();

            for (int y = 0; y < texture.Bounds.Height; y += tileSize)
            {
                for (int x = 0; x < texture.Bounds.Width; x += tileSize)
                {
                    rectangles.Add(new Rectangle(x, y, tileSize, tileSize));
                }
            }
        }

        public Rectangle SourceRectangle(int index)
        {
            return rectangles[index];
        }

        public Texture2D Texture
        {
            get;
            private set;
        }

        public int TileSize
        {
            get;
            private set;
        }
    }
}
