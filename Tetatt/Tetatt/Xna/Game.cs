using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Tao.Sdl;
using Tao.OpenAl;
using Tao.DevIl;
using System.Runtime.InteropServices;
using System.Linq;

namespace Microsoft.Xna.Framework
{
	public class Game : IDisposable
	{
		private GraphicsDeviceManager graphicsDeviceManager;
		private bool running;
		
		public GameComponentCollection Components { get; set; }
		public GameServiceContainer Services { get; set; }
		public ContentManager Content { get; set; }
		public GraphicsDevice GraphicsDevice { get { return graphicsDeviceManager.GraphicsDevice; } }
		
		public Game()
		{
			graphicsDeviceManager = null;
			running = false;
			
			Components = new GameComponentCollection();
			Services = new GameServiceContainer();
			Content = new ContentManager(Services);
		}
		
		public void Run()
		{
			if (System.IO.File.Exists("/System/Library/Frameworks/AppKit.framework/AppKit")) {
				// Unfortunately, SDL on OSX doesn't make the process a gui process,
				// so we try to do that here
				try {
					IntPtr psn = IntPtr.Zero;
					GetCurrentProcess (ref psn);
					TransformProcessType (ref psn, 1);
					SetFrontProcess (ref psn);
				} catch (Exception ex) {
					Console.WriteLine(ex); 
				}
			}

			Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
			Alut.alutInit();
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
            foreach (GameComponent c in Components)
            {
                c.Initialize();
            }
            Components.ComponentAdded += delegate(object sender, GameComponentCollectionEventArgs e) {
                e.GameComponent.Initialize();
            };
		}
		
		protected virtual void LoadContent()
		{
		}
		
		protected virtual void UnloadContent()
		{
		}
		
		protected virtual void Update(GameTime gameTime)
		{
			// TODO: Store sorted/filtered collection and update only on changes...
			foreach (IUpdateable c in Components.OfType<IUpdateable>().Where(c => c.Enabled).OrderBy(c => c.UpdateOrder)) {
				c.Update(gameTime);
			}
		}
		
		protected virtual void Draw(GameTime gameTime)
		{
			// TODO: Store sorted/filtered collection and update only on changes...
			foreach (IDrawable c in Components.OfType<IDrawable>().Where(c => c.Visible).OrderBy(c => c.DrawOrder)) {
				c.Draw(gameTime);
			}
		}
		
		public void Dispose()
		{
			Alut.alutExit();
			Sdl.SDL_Quit();
		}
		
		[DllImport ("/System/Library/Frameworks/AppKit.framework/AppKit")]
		private static extern void GetCurrentProcess (ref IntPtr psn);
		[DllImport ("/System/Library/Frameworks/AppKit.framework/AppKit")]
		private static extern void TransformProcessType (ref IntPtr psn, uint type);
		[DllImport ("/System/Library/Frameworks/AppKit.framework/AppKit")]
		private static extern void SetFrontProcess (ref IntPtr psn);
	}
}
