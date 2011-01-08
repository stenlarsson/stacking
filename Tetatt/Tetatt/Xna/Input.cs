using System;
using OpenTK.Input;

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
        public bool IsButtonUp (Buttons button) { return true; }
		public bool IsButtonDown (Buttons button) { return false; }
	}
	public class GamePad {
		public static GamePadState GetState(PlayerIndex index) { return new GamePadState(); }
	}
	public enum Keys {
		A = Key.A,
		D = Key.D,		
		W = Key.W,
		S = Key.S,
        Escape = Key.Escape,
        LeftControl = Key.ControlLeft,
		LeftShift = Key.ShiftLeft,
        Up = Key.Up,
        Left = Key.Left,
        Down = Key.Down,
        Right = Key.Right,
        RightControl = Key.ControlRight,
        RightShift = Key.ShiftRight,
		Enter = Key.Enter,
	}
	public struct KeyboardState {
        internal bool[] keystate;
        public bool IsKeyUp(Keys key)
        {
            return !IsKeyDown(key);
        }
		public bool IsKeyDown(Keys key)
        {
            return keystate[(int)key];
		}
	}
	public class Keyboard {
        internal static bool[] keys = new bool[Enum.GetValues(typeof(Key)).Length];

		public static KeyboardState GetState() {
			return new KeyboardState() {
                keystate = (bool[])keys.Clone()
			};
		}
	}
}
