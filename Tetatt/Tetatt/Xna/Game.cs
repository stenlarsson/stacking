using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Microsoft.Xna.Framework
{
	public class Game : IDisposable
	{
		private GraphicsDeviceManager graphicsDeviceManager;
		private bool running;
		
		public GameServiceContainer Services { get; set; }
		public ContentManager Content { get; set; }
		public GraphicsDevice GraphicsDevice { get { return graphicsDeviceManager.GraphicsDevice; } }
		
		public Game()
		{
			graphicsDeviceManager = null;
			running = false;
			
			Services = new GameServiceContainer();
			Content = new ContentManager(Services);
		}
		
		public void Run()
		{
			graphicsDeviceManager = (GraphicsDeviceManager)Services.GetService(typeof(GraphicsDeviceManager));
			graphicsDeviceManager.CreateDevice();
			Initialize();
			LoadContent();
			
			
			running = true;
			while(running)
			{
				int ticks = Environment.TickCount;
				GameTime gameTime = new GameTime();
				Update(gameTime);
				graphicsDeviceManager.BeginDraw();
				Draw(gameTime);
				graphicsDeviceManager.EndDraw();
				
				int sleepTicks = 16 + ticks - Environment.TickCount;
				if(sleepTicks > 0)
				{
					Thread.Sleep(sleepTicks);
				}
			}
		}
		
		public void Exit()
		{
			running = false;
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void LoadContent()
		{
		}
		
		protected virtual void UnloadContent()
		{
		}
		
		protected virtual void Update(GameTime gameTime)
		{
		}
		
		protected virtual void Draw(GameTime gameTime)
		{
		}
		
		public void Dispose()
		{
		}
	}
}
