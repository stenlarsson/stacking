using System;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Microsoft.Xna.Framework
{
	public class Game : IDisposable
	{
		private GraphicsDeviceManager graphicsDeviceManager;
		
		public GameServiceContainer Services { get; set; }
		public Content.ContentManager Content { get; set; }
		public GraphicsDevice GraphicsDevice { get { return graphicsDeviceManager.GraphicsDevice; } }
		
		public Game()
		{
		}
		
		public void Run()
		{
			graphicsDeviceManager = (GraphicsDeviceManager)Services.GetService(typeof(GraphicsDeviceManager));
			graphicsDeviceManager.CreateDevice();
			Initialize();
			LoadContent();
			
			int ticks = Environment.TickCount;
			
			while(true)
			{
				GameTime gameTime = new GameTime();
				Update(gameTime);
				graphicsDeviceManager.BeginDraw();
				Draw(gameTime);
				graphicsDeviceManager.EndDraw();
				
				int newticks = Environment.TickCount;
				Thread.Sleep(16 + ticks - newticks);
			}
		}
		
		public void Exit()
		{
			System.Environment.Exit(0);
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
