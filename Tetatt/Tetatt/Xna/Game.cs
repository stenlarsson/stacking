using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Tao.Sdl;
using Tao.DevIl;

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
			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Il.ilInit();
			Ilu.iluInit();

			graphicsDeviceManager = (GraphicsDeviceManager)Services.GetService(typeof(GraphicsDeviceManager));
			graphicsDeviceManager.CreateDevice();
			Initialize();
			LoadContent();
			
			
			GameTime gameTime = new GameTime();
			running = true;
			while(running)
			{
				int ticks = Environment.TickCount;
				Sdl.SDL_Event sdlEvent;
				if (Sdl.SDL_PollEvent(out sdlEvent) != 0)
				{
					if (sdlEvent.type == Sdl.SDL_QUIT)
					{
						Exit();
						break;
					}
				}
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
			Sdl.SDL_Quit();
		}
	}
}
