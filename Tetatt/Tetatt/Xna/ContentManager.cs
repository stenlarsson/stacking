using System;
using Microsoft.Xna.Framework.Graphics;

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
			return default(T);
		}
	}
}

