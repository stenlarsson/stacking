using System;
using Tao.Sdl;

namespace Microsoft.Xna.Framework.Input {
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
		public ButtonState Back { get { return ButtonState.Released; } }
	}
	public struct GamePadState {
		public GamePadButtons Buttons { get { return new GamePadButtons(); } }
		public bool IsButtonDown (Buttons button) { return false; }
	}
	public class GamePad {
		public static GamePadState GetState(PlayerIndex index) { return new GamePadState(); }
	}
	public enum Keys {
		A = Sdl.SDLK_a,
		D = Sdl.SDLK_d,		
		W = Sdl.SDLK_w,
		S = Sdl.SDLK_s,
		Escape = Sdl.SDLK_ESCAPE,
		LeftControl = Sdl.SDLK_LCTRL,
		LeftShift = Sdl.SDLK_LSHIFT
	}
	public struct KeyboardState {
		internal byte[] keystate;
		public bool IsKeyDown(Keys key) {
			return keystate[(int)key] != 0;
		}
	}
	public class Keyboard {
		public static KeyboardState GetState() {
			int dummy;
			return new KeyboardState() {
				keystate = Sdl.SDL_GetKeyState(out dummy)
			};
		}
	}
}
