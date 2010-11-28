using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Content
{
	public class ContentManager
	{
		private IServiceProvider serviceProvider;
		public IServiceProvider ServiceProvider { get { return serviceProvider; } }
		public string RootDirectory { get; set; }
		
		
		public ContentManager(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public virtual T Load<T> (string assetName)
		{
			if(typeof(T)==typeof(Texture2D))
			{
				return (T)(object)new Texture2D(string.Format("../../../TetattContent/{0}.png", assetName));
			}
			else if (typeof(T)==typeof(SoundEffect))
			{
				return (T)(object)new SoundEffect(string.Format("../../../TetattContent/{0}.wav", assetName));
			}
			return default(T);
		}
	}
}

