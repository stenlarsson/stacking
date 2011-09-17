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

        Dictionary<MenuInput, Buttons[]> menuButtons;
        Dictionary<MenuInput, Keys[]>[] menuKeys;

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

            keyMappings[0].Add(Keys.Up, PlayerInput.Up);
            keyMappings[0].Add(Keys.Down, PlayerInput.Down);
            keyMappings[0].Add(Keys.Left, PlayerInput.Left);
            keyMappings[0].Add(Keys.Right, PlayerInput.Right);
            keyMappings[0].Add(Keys.RightControl, PlayerInput.Swap);
            keyMappings[0].Add(Keys.RightShift, PlayerInput.Raise);

            keyMappings[1].Add(Keys.W, PlayerInput.Up);
            keyMappings[1].Add(Keys.S, PlayerInput.Down);
            keyMappings[1].Add(Keys.A, PlayerInput.Left);
            keyMappings[1].Add(Keys.D, PlayerInput.Right);
            keyMappings[1].Add(Keys.LeftControl, PlayerInput.Swap);
            keyMappings[1].Add(Keys.LeftShift, PlayerInput.Raise);

            // Note: We don't have any mappings for player three and four

            heldButtons = new Dictionary<Buttons, int>[] {
                new Dictionary<Buttons, int>(),
                new Dictionary<Buttons, int>(),
                new Dictionary<Buttons, int>(),
                new Dictionary<Buttons, int>(),
            };
            heldKeys = new Dictionary<Keys, int>();

            menuButtons = new Dictionary<MenuInput, Buttons[]>();
            AddMenuButtons(MenuInput.Up, Buttons.DPadUp, Buttons.LeftThumbstickUp);
            AddMenuButtons(MenuInput.Down, Buttons.DPadDown, Buttons.LeftThumbstickDown);
            AddMenuButtons(MenuInput.Left, Buttons.DPadLeft, Buttons.LeftThumbstickLeft);
            AddMenuButtons(MenuInput.Right, Buttons.DPadRight, Buttons.LeftThumbstickRight);
            AddMenuButtons(MenuInput.Select, Buttons.A, Buttons.Start);
            AddMenuButtons(MenuInput.Cancel, Buttons.B, Buttons.Back);
            AddMenuButtons(MenuInput.Toggle, Buttons.X);
            AddMenuButtons(MenuInput.Pause, Buttons.Start, Buttons.Back);

            menuKeys = new Dictionary<MenuInput, Keys[]>[MaxInputs];
            for (int i = 0; i < menuKeys.Length; i++)
                menuKeys[i] = new Dictionary<MenuInput, Keys[]>();
            AddMenuKeys(PlayerIndex.One, MenuInput.Up, Keys.Up);
            AddMenuKeys(PlayerIndex.One, MenuInput.Down, Keys.Down);
            AddMenuKeys(PlayerIndex.One, MenuInput.Left, Keys.Left);
            AddMenuKeys(PlayerIndex.One, MenuInput.Right, Keys.Right);
            AddMenuKeys(PlayerIndex.One, MenuInput.Select, Keys.RightControl);
            AddMenuKeys(PlayerIndex.One, MenuInput.Cancel, Keys.Escape);
            AddMenuKeys(PlayerIndex.One, MenuInput.Toggle, Keys.RightShift);
            AddMenuKeys(PlayerIndex.One, MenuInput.Pause, Keys.Escape);

            AddMenuKeys(PlayerIndex.Two, MenuInput.Up, Keys.W);
            AddMenuKeys(PlayerIndex.Two, MenuInput.Down, Keys.S);
            AddMenuKeys(PlayerIndex.Two, MenuInput.Left, Keys.A);
            AddMenuKeys(PlayerIndex.Two, MenuInput.Right, Keys.D);
            AddMenuKeys(PlayerIndex.Two, MenuInput.Select, Keys.LeftControl);
            AddMenuKeys(PlayerIndex.Two, MenuInput.Cancel, Keys.Escape);
            AddMenuKeys(PlayerIndex.Two, MenuInput.Toggle, Keys.LeftShift);
            AddMenuKeys(PlayerIndex.Two, MenuInput.Pause, Keys.Escape);
        }

        void AddMenuButtons(MenuInput input, params Buttons[] buttons)
        {
            menuButtons.Add(input, buttons);
        }

        void AddMenuKeys(PlayerIndex player, MenuInput input, params Keys[] keys)
        {
            menuKeys[(int)player].Add(input, keys);
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
        /// Checks for the specified menu input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it,
        /// otherwise the value is unspecified.
        /// </summary>
        public bool IsMenuInput(MenuInput input, PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                foreach (var item in menuKeys[(int)controllingPlayer][input])
                    if (IsNewKeyPress(item, controllingPlayer, out playerIndex))
                        return true;

                foreach (var item in menuButtons[input])
                    if (IsNewButtonPress(item, controllingPlayer, out playerIndex))
                        return true;            
            }
            else
            {
                Keys[] keys;
                for (int i = 0; i < MaxInputs; i++)
                    if (menuKeys[i].TryGetValue(input, out keys))
                        foreach (var item in keys)
                            if (IsNewKeyPress(item, (PlayerIndex)i, out playerIndex))
                                return true;

                for (int i = 0; i < MaxInputs; i++)
                    foreach (var item in menuButtons[input])
                        if (IsNewButtonPress(item, (PlayerIndex)i, out playerIndex))
                            return true;
            }
            playerIndex = PlayerIndex.One; 
            return false;
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
            return IsMenuInput(MenuInput.Select, controllingPlayer, out playerIndex);
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
            return IsMenuInput(MenuInput.Cancel, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsMenuInput(MenuInput.Up, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsMenuInput(MenuInput.Down, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuLeft(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsMenuInput(MenuInput.Left, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuRight(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsMenuInput(MenuInput.Right, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu toggle" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuToggle(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsMenuInput(MenuInput.Toggle, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsMenuInput(MenuInput.Pause, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Returns the gameplay input for specified player. Handles key repeat and button mappings.
        /// Must be called exactly once each frame for each player. This is separate method because
        /// we don't want key repeat for the menus.
        /// </summary>
        public PlayerInput GetPlayerInput(PlayerIndex player)
        {
            // TODO fix so that this method can be called more than once each frame

            int playerInt = (int)player;

            KeyboardState keyboardState = CurrentKeyboardStates[playerInt];
            GamePadState gamePadState = CurrentGamePadStates[playerInt];

            // Instead of using LeftThumbstickUp etc we check which axis is largest
            // to avoid diagonals. We also use a very big dead zone to handle the
            // stick swinging in the other direction when released, but with this
            // dead zone diagonals is not really a problem...
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

            return PlayerInput.None;
        }

    }
}
