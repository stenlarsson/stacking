using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tetatt
{
    /// <summary>
    /// This represents the state of controller and keyboard input. It keeps
    /// track of if a button is down and if it also was down last frame.
    /// The left analog stick will emulate a four-way D-Pad.
    /// Call Update() each frame.
    /// </summary>
    class InputState
    {
        /// <summary>
        /// Supported number of controllers.
        /// Xbox only supports up to four controllers.
        /// </summary>
        const int NumGamepads = 4;

        /// <summary>
        /// State of game pads last frame.
        /// </summary>
        GamePadState[] oldGamePadState;
        /// <summary>
        /// State of game pads this frame.
        /// </summary>
        GamePadState[] gamePadState;

        /// <summary>
        /// State of emulated D-Pad last frame.
        /// </summary>
        Buttons?[] oldEmulatedDPadState;
        /// <summary>
        /// State of emulated D-Pad this frame.
        /// </summary>
        Buttons?[] emulatedDPadState;

        /// <summary>
        /// State of keyboard last frame.
        /// </summary>
        KeyboardState oldKeyboardState;
        /// <summary>
        /// State of keyboard this frame.
        /// </summary>
        KeyboardState keyboardState;

        /// <summary>
        /// Create new InputState.
        /// </summary>
        public InputState()
        {
            oldGamePadState = new GamePadState[NumGamepads];
            gamePadState = new GamePadState[NumGamepads];
            oldEmulatedDPadState = new Buttons?[NumGamepads];
            emulatedDPadState = new Buttons?[NumGamepads];

            // Update twice to get initial state of last frame
            UpdateState();
            UpdateState();
        }

        /// <summary>
        /// Call each frame to update state.
        /// </summary>
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

                oldEmulatedDPadState[i] = emulatedDPadState[i];

                // Update emulated D-Pad. Check which axis is the longest to
                // avoid diagonals. Note that dead zone is provided by
                // GamePad.GetState()
                emulatedDPadState[i] = null;
                Vector2 left = gamePadState[i].ThumbSticks.Left;
                if (Math.Abs(left.X) > Math.Abs(left.Y))
                {
                    if (left.X < 0)
                    {
                        emulatedDPadState[i] = Buttons.DPadLeft;
                    }
                    else if (left.X > 0)
                    {
                        emulatedDPadState[i] = Buttons.DPadRight;
                    }
                }
                else
                {
                    if (left.Y > 0)
                    {
                        emulatedDPadState[i] = Buttons.DPadUp;
                    }
                    else if (left.Y < 0)
                    {
                        emulatedDPadState[i] = Buttons.DPadDown;
                    }
                }
            }

            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// If key is pressed down, regardless of what it was last frame.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if key is down</returns>
        public bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// If key is pressed down, but wasn't last frame.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if key is down, but was up last frame</returns>
        public bool IsKeyDownThisFrame(Keys key)
        {
            return oldKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// If controller button is pressed down, regardless of what it was last
        /// frame.
        /// </summary>
        /// <param name="index">Controller number (1-4)</param>
        /// <param name="button">Button to check</param>
        /// <returns>True if button is down</returns>
        public bool IsButtonDown(int index, Buttons button)
        {
            return gamePadState[index].IsButtonDown(button) ||
                emulatedDPadState[index] == button;
        }

        /// <summary>
        /// If controller button is pressed down, but wasn't last frame.
        /// </summary>
        /// <param name="index">Controller number (1-4)</param>
        /// <param name="button">Button to check</param>
        /// <returns>>True if button is down, but was up last frame</returns>
        public bool IsButtonDownThisFrame(int index, Buttons button)
        {
            return oldGamePadState[index].IsButtonUp(button) &&
                gamePadState[index].IsButtonDown(button) ||
                oldEmulatedDPadState[index] != button &&
                emulatedDPadState[index] == button;
        }
    }
}
