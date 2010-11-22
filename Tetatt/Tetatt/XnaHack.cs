using System;
#if WINDOWS || XBOX
#else
namespace Microsoft.Xna.Framework
{
	namespace Graphics {
		public struct Color {
			public Color(byte r, byte g, byte b, byte a) {
				R = r; G = g; B = b; A = a;
			}
			public readonly byte R, G, B, A;
			public static Color Black { get { return new Color(0,0,0,255); } }
			public static Color White { get { return new Color(255,255,255,255); } }
			public static Color DarkGray { get { return new Color(169,169,169,255); } }
		}
		public class GraphicsDevice {
			public static void Clear(Color color) {}
			public Rectangle ScissorRectangle { get; set; }
		}
		public class GraphicsDeviceManager {
			public GraphicsDeviceManager(Game game) {}
			public int PreferredBackBufferWidth { get; set; }
			public int PreferredBackBufferHeight { get; set; }
		}
		public enum SpriteSortMode { Deferred }
		public class BlendState {}
		public class SamplerState {}
		public class DepthStencilState {}
		public class RasterizerState {
			public bool ScissorTestEnable { get; set; }
		}
		public class SpriteBatch {
			public GraphicsDevice GraphicsDevice { get { return null; } }
			public SpriteBatch(GraphicsDevice device) {}
			public void Begin () {}
			public void Begin (SpriteSortMode sortMode, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null) {}
			public void End () {}
			public void Draw (Texture2D texture, Vector2 position, Color color) {}
			public void Draw (Texture2D texture, Rectangle destinationRectangle, Nullable<Rectangle> sourceRectangle, Color color) {}

		}
		public class Texture2D {
			public Rectangle Bounds { get { return new Rectangle(0,0,0,0); } }
		}
	}
	namespace Audio {}
	namespace GamerServices {}
	namespace Content {
		public class ContentManager {
			public string RootDirectory { get; set; }
			public virtual T Load<T> (string assetName) { return default (T); }
		}
	}
	namespace Input {
		[FlagsAttribute]
		public enum Buttons {
			A, B, Back, BigButton, DPadDown, DPadLeft, DPadRight, DPadUp, LeftShoulder, LeftStick,
			LeftThumbstickDown, LeftThumbstickLeft, LeftThumbstickRight, LeftThumbstickUp,
			LeftTrigger, RightShoulder, RightStick, RightThumbstickDown, RightThumbstickLeft,
			RightThumbstickRight, RightThumbstickUp, RightTrigger, Start, X, Y
		}
		public enum ButtonState {
			Pressed, Released
		}
		public struct GamePadButtons {
			public ButtonState Back { get { return ButtonState.Pressed; } }
		}
		public struct GamePadState {
			public GamePadButtons Buttons { get { return new GamePadButtons(); } }
			public bool IsButtonDown (Buttons button) { return false; }
		}
		public class GamePad {
			public static GamePadState GetState(PlayerIndex index) { return new GamePadState(); }
		}
		public enum Keys {
			A, Add, Apps, Attn, B, Back, BrowserBack, BrowserFavorites, BrowserForward, BrowserHome, BrowserRefresh, BrowserSearch, BrowserStop, C, CapsLock, ChatPadGreen, ChatPadOrange, Crsel, D, D0, D1, D2, D3, D4, D5, D6, D7, D8, D9, Decimal, Delete, Divide, Down, E, End, Enter, EraseEof, Escape, Execute, Exsel, F, F1, F10, F11, F12, F13, F14, F15, F16, F17, F18, F19, F2, F20, F21, F22, F23, F24, F3, F4, F5, F6, F7, F8, F9, G, H, Help, Home, I, ImeConvert, ImeNoConvert, Insert, J, K, Kana, Kanji, L, LaunchApplication1, LaunchApplication2, LaunchMail, Left, LeftAlt, LeftControl, LeftShift, LeftWindows, M, MediaNextTrack, MediaPlayPause, MediaPreviousTrack, MediaStop, Multiply, N, None, NumLock, NumPad0, NumPad1, NumPad2, NumPad3, NumPad4, NumPad5, NumPad6, NumPad7, NumPad8, NumPad9, O, Oem8, OemAuto, OemBackslash, OemClear, OemCloseBrackets, OemComma, OemCopy, OemEnlW, OemMinus, OemOpenBrackets, OemPeriod, OemPipe, OemPlus, OemQuestion, OemQuotes, OemSemicolon, OemTilde, P, Pa1, PageDown, PageUp, Pause, Play, Print, PrintScreen, ProcessKey, Q, R, Right, RightAlt, RightControl, RightShift, RightWindows, S, Scroll, Select, SelectMedia, Separator, Sleep, Space, Subtract, T, Tab, U, Up, V, VolumeDown, VolumeMute, VolumeUp, W, X, Y, Z, Zoom
		}
		public struct KeyboardState {
			public bool IsKeyDown(Keys key) { return false; }
		}
		public class Keyboard {
			public static KeyboardState GetState() { return new KeyboardState(); }
		}
	}
	namespace Media {}

	public class GameTime {}
	public class Game : IDisposable
	{
		public Content.ContentManager Content { get; set; }
		public Graphics.GraphicsDevice GraphicsDevice { get; set; }
		public void Exit () {}
		public void Run () {}
		protected virtual void Initialize() {}
		protected virtual void LoadContent() {}
		protected virtual void UnloadContent() {}
		protected virtual void Update(GameTime gameTime) {}
		protected virtual void Draw(GameTime gameTime) {}
		public void Dispose() {}
	}
	public enum PlayerIndex { One, Two, Three, Four }
	public struct Rectangle {
		public Rectangle(int x, int y, int width, int height) {
			X = x; Y = y; Width = width; Height = height;
		}
		public int X, Y, Width, Height;
	}
	public struct Vector2 {
		public Vector2(float x, float y) {
			X = x; Y = y;
		}
		public float X, Y;
	}
}
#endif