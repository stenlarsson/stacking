using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Microsoft.Xna.Framework.Content
{
    public class ContentManager
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public string RootDirectory { get; set; }

        public ContentManager(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
        {
            ServiceProvider = serviceProvider;
            RootDirectory = rootDirectory;
        }

        public virtual T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                return (T)(object)Texture2D.FromPath(string.Format("../../../TetattContent/{0}.png", assetName));
            }
            else if (typeof(T) == typeof(Song))
            {
                return (T)(object)new Song(string.Format("../../../TetattContent/{0}.wav", assetName));
            }
            else if (typeof(T) == typeof(SoundEffect))
            {
                return (T)(object)new SoundEffect(string.Format("../../../TetattContent/{0}.wav", assetName));
            }
            else if (typeof(T)==typeof(SpriteFont))
            {
                return (T)(object)new SpriteFont(
                    Texture2D.FromPath(string.Format("../../../TetattContent/{0}.png", assetName), System.Drawing.Color.Magenta),
                    XDocument.Load(string.Format("../../../TetattContent/{0}.xml", assetName)).Root.Elements("character").ToDictionary(
                        e => (char)int.Parse(e.Attribute("key").Value),
                        e => new Rectangle(
                                 int.Parse(e.Element("x").Value),
                                 int.Parse(e.Element("y").Value),
                                 int.Parse(e.Element("width").Value),
                                 int.Parse(e.Element("height").Value))
                   )
                );
            }
            return default(T);
        }

        public void Unload()
        {
        }
    }
}

