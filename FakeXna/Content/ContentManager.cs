using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Microsoft.Xna.Framework.Content
{
    public class ContentManager
    {
        Dictionary<string, object> loaded = new Dictionary<string, object>();
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

        Stream GetResource(string name, string type)
        {
            return new FileStream(
                String.Format("{0}/{1}.{2}", RootDirectory, name, type),
                FileMode.Open);
        }

        object Cache(string assetName, object obj)
        {
            loaded.Add(assetName, obj);
            return obj;
        }

        public virtual T Load<T>(string assetName)
        {
            object val;
            if (loaded.TryGetValue(assetName, out val))
            {
                try {
                    return (T)val;
                } catch (InvalidCastException ice) {
                    throw new ContentLoadException(
                         string.Format(
                             "Couldn't load asset '{0}' of type '{1}'",
                             assetName, typeof(T).Name), ice);
                }
            }

            if (typeof(T) == typeof(Texture2D))
            {
                using(var stream = GetResource(assetName, "png"))
                    return (T)Cache(assetName, Texture2D._FromPngStream(stream));
            }
            else if (typeof(T) == typeof(Song))
            {
                var di = new DirectoryInfo(RootDirectory);
                Exception finale = null;
                try {
                    using (var stream = GetResource(assetName, "wav"))
                        return (T)Cache(assetName, Song._FromWavStream(stream));
                } catch (Exception e) {
                    finale = e;
                }
                foreach (var fi in di.EnumerateFiles(string.Format("{0}.*", assetName)))
                {
                    try {
                        using (var stream = GetResource(assetName, fi.Extension.Substring(1)))
                            return (T)Cache(assetName, Song._FromGenericStream(stream));
                    } catch (Exception e) {
                        // Ignore all errors but last one
                        finale = e;
                    }
                }
                throw finale; // If we couldn't find anything that loads.
            }
            else if (typeof(T) == typeof(SoundEffect))
            {
                using (var stream = GetResource(assetName, "wav"))
                    return (T)Cache(assetName, SoundEffect._FromWavStream(stream));
            }
            else if (typeof(T)==typeof(SpriteFont))
            {
                using (var pngStream = GetResource(assetName, "png"))
                    using (var xmlStream = GetResource(assetName, "xml"))
                        return (T)Cache(assetName, SpriteFont._FromTextureAndXmlStream(
                            Texture2D._FromPngStream(pngStream, System.Drawing.Color.Magenta),
                            xmlStream));
            }
            return default(T);
        }

        public void Unload()
        {
        }
    }
}

