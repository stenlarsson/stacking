using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using System.Runtime.InteropServices;
using System.Linq;

namespace Microsoft.Xna.Framework
{
    public class Game : IDisposable
    {
        private GameWindow gameWindow;
        private GraphicsDeviceManager graphicsDeviceManager;
        private AudioContext audioContext = new AudioContext();

        public GameComponentCollection Components { get; set; }
        public GameServiceContainer Services { get; set; }
        public ContentManager Content { get; set; }
        public bool IsActive { get; set; }
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDeviceManager.GraphicsDevice; }
        }
 
        public Game()
        {
            graphicsDeviceManager = null;
            
            Components = new GameComponentCollection();
            Services = new GameServiceContainer();
            Content = new ContentManager(Services);
            IsActive = true;
            
            gameWindow = new GameWindow(1280, 720, GraphicsMode.Default, "Tetatt");
            gameWindow.VSync = VSyncMode.On;
            gameWindow.Keyboard.KeyDown += (s, e) => Input.Keyboard.keys[(int)e.Key] = true;
            gameWindow.Keyboard.KeyUp += (s, e) => Input.Keyboard.keys[(int)e.Key] = false;
            gameWindow.Load += (s, e) => LoadContent();
            gameWindow.Resize += (s, e) => GraphicsDevice.Viewport.windowRectangle = gameWindow.ClientRectangle;
            gameWindow.UpdateFrame += OnUpdateFrame;
            gameWindow.RenderFrame += OnRenderFrame;
        }

        public void Run()
        {
            graphicsDeviceManager = (GraphicsDeviceManager)Services.GetService(typeof(GraphicsDeviceManager));
            gameWindow.Width = graphicsDeviceManager.PreferredBackBufferWidth;
            gameWindow.Height = graphicsDeviceManager.PreferredBackBufferHeight;
            graphicsDeviceManager.CreateDevice();
            Initialize();
            LoadContent();
            
            gameWindow.Run(60.0, 60.0);
        }

        public void Exit()
        {
            gameWindow.Exit();
        }

        private void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            GameTime gameTime = new GameTime(e.Time);
            GameTime.totalGameTime += gameTime.ElapsedGameTime;
            Update(gameTime);
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GameTime gameTime = new GameTime(e.Time);
            Draw(gameTime);
            
            ErrorCode error = GL.GetError();
            while (error != ErrorCode.NoError)
            {
                Console.WriteLine("OpenGL error {0}", error);
                error = GL.GetError();
            }
            
            gameWindow.SwapBuffers();
        }

        protected virtual void Initialize()
        {
            foreach (GameComponent c in Components)
            {
                c.Initialize();
            }
            Components.ComponentAdded += ComponentAdded;
            Components.ComponentRemoved += ComponentRemoved;
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
            foreach (IUpdateable c in Components.OfType<IUpdateable>().Where(c => c.Enabled).OrderBy(c => c.UpdateOrder))
            {
                c.Update(gameTime);
            }
        }

        protected virtual void Draw(GameTime gameTime)
        {
            // TODO: Store sorted/filtered collection and update only on changes...
            foreach (IDrawable c in Components.OfType<IDrawable>().Where(c => c.Visible).OrderBy(c => c.DrawOrder))
            {
                c.Draw(gameTime);
            }
        }

        public void Dispose()
        {
            gameWindow.Dispose();
            audioContext.Dispose();
        }

        public void ResetElapsedTime()
        {
            // TODO implement
        }

        private void ComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            e.GameComponent.Initialize();
            ((GameComponent)e.GameComponent).Disposed += ComponentDisposed;
        }

        private void ComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            ((GameComponent)e.GameComponent).Disposed -= ComponentDisposed;
        }

        private void ComponentDisposed(object sender, EventArgs e)
        {
            Components.Remove((GameComponent)sender);
        }
    }
}
