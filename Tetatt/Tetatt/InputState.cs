using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tetatt
{
    class InputState
    {
        const int NumGamepads = 4;

        GamePadState[] oldGamePadState;
        GamePadState[] gamePadState;

        KeyboardState oldKeyboardState;
        KeyboardState keyboardState;

        public InputState()
        {
            oldGamePadState = new GamePadState[NumGamepads];
            gamePadState = new GamePadState[NumGamepads];
            UpdateState();
            UpdateState();
        }

        public void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            for (int i = 0; i < NumGamepads; i++)
            {
                oldGamePadState[i] = gamePadState[i];
                gamePadState[i] = GamePad.GetState((PlayerIndex)i);
            }

            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public bool IsKeyDownThisFrame(Keys key)
        {
            return oldKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        public bool IsButtonDown(int index, Buttons button)
        {
            return gamePadState[index].IsButtonDown(button);
        }

        public bool IsButtonDownThisFrame(int index, Buttons button)
        {
            return oldGamePadState[index].IsButtonUp(button) && gamePadState[index].IsButtonDown(button);
        }
    }
}
