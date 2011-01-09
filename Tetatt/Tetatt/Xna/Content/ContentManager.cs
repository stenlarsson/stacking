using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Content
{
    public class ContentManager
    {
        private IServiceProvider serviceProvider;
        public IServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
        }
        public string RootDirectory { get; set; }


        public ContentManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public virtual T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                return (T)(object)new Texture2D(string.Format("../../../TetattContent/{0}.png", assetName));
            }

            else if (typeof(T) == typeof(SoundEffect))
            {
                return (T)(object)new SoundEffect(string.Format("../../../TetattContent/{0}.wav", assetName));
            }
            else if (typeof(T)==typeof(SpriteFont))
            {
                return (T)(object)new SpriteFont(
                    new Texture2D(string.Format("../../../TetattContent/{0}.png", assetName)),
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
    }
}

