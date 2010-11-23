using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework
{
	public class GameServiceContainer
	{
		public Dictionary<Type, Object> services;
		
		public GameServiceContainer()
		{
			services = new Dictionary<Type, Object>();
		}
	
		public void AddService(Type type, Object provider)
		{
			services[type] = provider;
		}
		
		public Object GetService(Type type)
		{
			return services[type];
		}
	}
}
