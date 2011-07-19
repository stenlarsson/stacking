#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System;
using Tetatt.GamePlay;
#endregion

namespace Tetatt.Screens
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public const int MaxInputs = 4;
        private const int KeyRepeatDelay = 8; // frames
        private const int KeyRepeatRate = 2;


        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        private Dictionary<Buttons, PlayerInput> buttonMapping;
        private Dictionary<Keys, PlayerInput>[] keyMappings;

        private Dictionary<Buttons, int>[] heldButtons;
        private Dictionary<Keys, int> heldKeys;

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];

            // Controller button mapping (same for all controllers)
            buttonMapping = new Dictionary<Buttons, PlayerInput>();
            buttonMapping.Add(Buttons.DPadUp, PlayerInput.Up);
            buttonMapping.Add(Buttons.DPadDown, PlayerInput.Down);
            buttonMapping.Add(Buttons.DPadLeft, PlayerInput.Left);
            buttonMapping.Add(Buttons.DPadRight, PlayerInput.Right);
            //buttonMapping.Add(Buttons.LeftThumbstickUp, PlayerInput.Up);
            //buttonMapping.Add(Buttons.LeftThumbstickDown, PlayerInput.Down);
            //buttonMapping.Add(Buttons.LeftThumbstickLeft, PlayerInput.Left);
            //buttonMapping.Add(Buttons.LeftThumbstickRight, PlayerInput.Right);
            buttonMapping.Add(Buttons.A, PlayerInput.Swap);
            buttonMapping.Add(Buttons.B, PlayerInput.Swap);
            buttonMapping.Add(Buttons.X, PlayerInput.Swap);
            buttonMapping.Add(Buttons.Y, PlayerInput.Swap);
            buttonMapping.Add(Buttons.LeftShoulder, PlayerInput.Raise);
            buttonMapping.Add(Buttons.RightShoulder, PlayerInput.Raise);
            buttonMapping.Add(Buttons.LeftTrigger, PlayerInput.Raise);
            buttonMapping.Add(Buttons.RightTrigger, PlayerInput.Raise);

            // Keyboard mapping (different for different players)
            keyMappings = new Dictionary<Keys, PlayerInput>[] {
                new Dictionary<Keys, PlayerInput>(),
                new Dictionary<Keys, PlayerInput>(),
                new Dictionary<Keys, PlayerInput>(),
                new Dictionary<Keys, PlayerInput>(),
            };

            keyMappings[0].Add(Keys.W, PlayerInput.Up);
            keyMappings[0].Add(Keys.S, PlayerInput.Down);
            keyMappings[0].Add(Keys.A, PlayerInput.Left);
            keyMappings[0].Add(Keys.D, PlayerInput.Right);
            keyMappings[0].Add(Keys.LeftControl, PlayerInput.Swap);
            keyMappings[0].Add(Keys.LeftShift, PlayerInput.Raise);

            keyMappings[1].Add(Keys.Up, PlayerInput.Up);
            keyMappings[1].Add(Keys.Down, PlayerInput.Down);
            keyMappings[1].Add(Keys.Left, PlayerInput.Left);
            keyMappings[1].Add(Keys.Right, PlayerInput.Right);
            keyMappings[1].Add(Keys.RightControl, PlayerInput.Swap);
            keyMappings[1].Add(Keys.RightShift, PlayerInput.Raise);

            // Note: We don't have any mappings for player three and four

            heldButtons = new Dictionary<Buttons, int>[] {
                new Dictionary<Buttons, int>(),
                new Dictionary<Buttons, int>(),
                new Dictionary<Buttons, int>(),
                new Dictionary<Buttons, int>(),
            };
            heldKeys = new Dictionary<Keys, int>();
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }

            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                     out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Returns the gameplay input for specified player. Handles key repeat and button mappings.
        /// Must be called exactly once each frame for each player.
        /// </summary>
        public PlayerInput? GetPlayerInput(PlayerIndex player)
        {
            // TODO fix so that this method can be called more than once each frame

            int playerInt = (int)player;

            KeyboardState keyboardState = CurrentKeyboardStates[playerInt];
            GamePadState gamePadState = CurrentGamePadStates[playerInt];

            // Instead of using LeftThumbstickUp etc we check which axis is largest
            // to avoid diagonals
            Buttons? thumbstick = null;
            Vector2 left = gamePadState.ThumbSticks.Left;
            if (Math.Abs(left.X) > Math.Abs(left.Y))
            {
                if (left.X < -0.75)
                {
                    thumbstick = Buttons.DPadLeft;
                }
                else if (left.X > 0.75)
                {
                    thumbstick = Buttons.DPadRight;
                }
            }
            else
            {
                if (left.Y > 0.75)
                {
                    thumbstick = Buttons.DPadUp;
                }
                else if (left.Y < -0.75)
                {
                    thumbstick = Buttons.DPadDown;
                }
            }

            var playerHeldButtons = heldButtons[playerInt];
            // Check controller buttons with key repeat
            foreach (KeyValuePair<Buttons, PlayerInput> kvp in buttonMapping)
            {
                if (gamePadState.IsButtonDown(kvp.Key) || thumbstick == kvp.Key)
                {
                    if (playerHeldButtons.ContainsKey(kvp.Key))
                    {
                        if (--playerHeldButtons[kvp.Key] < 0)
                        {
                            playerHeldButtons[kvp.Key] = KeyRepeatRate;
                            return kvp.Value;
                        }
                    }
                    else
                    {
                        playerHeldButtons[kvp.Key] = KeyRepeatDelay;
                        return kvp.Value;
                    }
                }
                else
                {
                    playerHeldButtons.Remove(kvp.Key);
                }
            }

            // Check keyboard keys with key repeat
            foreach (KeyValuePair<Keys, PlayerInput> kvp in keyMappings[playerInt])
            {
                if (keyboardState.IsKeyDown(kvp.Key))
                {
                    if (heldKeys.ContainsKey(kvp.Key))
                    {
                        if (--heldKeys[kvp.Key] < 0)
                        {
                            heldKeys[kvp.Key] = KeyRepeatRate;
                            return kvp.Value;
                        }
                    }
                    else
                    {
                        heldKeys[kvp.Key] = KeyRepeatDelay;
                        return kvp.Value;
                    }
                }
                else
                {
                    heldKeys.Remove(kvp.Key);
                }
            }

            return null;
        }

    }
}
