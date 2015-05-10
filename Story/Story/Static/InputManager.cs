using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    static class InputManager
    {

        private static GamePadState GamePadState;
        private static KeyboardState KeyboardState;
        private static GamePadState LastGamePadState;
        private static KeyboardState LastKeyboardState;
        private static MouseState MouseState;
        private static MouseState LastMouseState;

        public static Vector2 MouseCoords;

        // A table containing input pairs as well as the duration since they were last pressed
        private static Dictionary<InputPair, float> InputLookupTable = new Dictionary<InputPair, float>();
        private static Dictionary<InputPair, float> PreparedInputLookupTable = new Dictionary<InputPair, float>();
        private static InputPair PressedInput;

        public const float LargeInputDelay = 500f;
        public const float StandardInputDelay = 200f;
        public const float SmallInputDelay = 25f;
        public const float NoInputDelay = 0f;

        public const float ThumbStickPressThreshold = 0.15f;
        public const float ThumbStickReleaseThreshold = 0.15f;

        public const float TriggerPressThreshold = 0.15f;
        public const float TriggerReleaseThreshold = 0.15f;

        // Dummy buttons for input delay caching for many-state input buttons or non-buttons
        private static Keys[] DummyLeftThumbStickKeysUp = { };
        private static Buttons[] DummyLeftThumbStickButtonsUp = { };
        private static Keys[] DummyLeftThumbStickKeysDown = { };
        private static Buttons[] DummyLeftThumbStickButtonsDown = { };
        private static Keys[] DummyLeftThumbStickKeysLeft = { };
        private static Buttons[] DummyLeftThumbStickButtonsLeft = { };
        private static Keys[] DummyLeftThumbStickKeysRight = { };
        private static Buttons[] DummyLeftThumbStickButtonsRight = { };

        private static Keys[] DummyRightThumbStickKeysUp = { };
        private static Buttons[] DummyRightThumbStickButtonsUp = { };
        private static Keys[] DummyRightThumbStickKeysDown = { };
        private static Buttons[] DummyRightThumbStickButtonsDown = { };
        private static Keys[] DummyRightThumbStickKeysLeft = { };
        private static Buttons[] DummyRightThumbStickButtonsLeft = { };
        private static Keys[] DummyRightThumbStickKeysRight = { };
        private static Buttons[] DummyRightThumbStickButtonsRight = { };

        private static Keys[] DummyLeftTriggerKeys = { };
        private static Buttons[] DummyLeftTriggerButtons = { };
        private static Keys[] DummyRightTriggerKeys = { };
        private static Buttons[] DummyRightTriggerButtons = { };

        private static Keys[] DummyMouseKeys = { };
        private static Buttons[] DummyMouseButtons = { };

        // Cardinal directions
        public static Keys[] LeftKeys = { Keys.A, Keys.Left };
        public static Keys[] RightKeys = { Keys.D, Keys.Right };
        public static Keys[] UpKeys = { Keys.W, Keys.Up };
        public static Keys[] DownKeys = { Keys.S, Keys.Down };
        public static Buttons[] LeftButtons = { Buttons.DPadLeft };
        public static Buttons[] RightButtons = { Buttons.DPadRight };
        public static Buttons[] UpButtons = { Buttons.DPadUp };
        public static Buttons[] DownButtons = { Buttons.DPadDown };


        // Alternative directional
        public static Keys[] AlternativeLeftKeys = { Keys.J };
        public static Keys[] AlternativeRightKeys = { Keys.L };
        public static Keys[] AlternativeUpKeys = { Keys.I };
        public static Keys[] AlternativeDownKeys = { Keys.K };
        public static Buttons[] AlternativeLeftButtons = { };
        public static Buttons[] AlternativeRightButtons = { };
        public static Buttons[] AlternativeUpButtons = { };
        public static Buttons[] AlternativeDownButtons = { };

        // Accept
        public static Keys[] AcceptKeys = { Keys.Enter, Keys.Space };
        public static Buttons[] AcceptButtons = { Buttons.A, Buttons.Start };

        public static Keys[] AcceptKeysNoMenu = { Keys.Enter };
        public static Buttons[] AcceptButtonsNoMenu = { Buttons.A };

        // Decline
        public static Keys[] DeclineKeys = { Keys.Escape, Keys.Back };
        public static Buttons[] DeclineButtons = { Buttons.B, Buttons.Back };

        // Extra buttons/keys
        public static Keys[] UtilityAKeys = { Keys.R };
        public static Buttons[] UtilityAButtons = { Buttons.Y };

        public static Keys[] UtilityBKeys = { Keys.F };
        public static Buttons[] UtilityBButtons = { Buttons.X };

        public static Buttons[] UtilityCButtons = { Buttons.DPadLeft };
        public static Buttons[] UtilityDButtons = { Buttons.DPadRight };
        public static Buttons[] UtilityEButtons = { Buttons.DPadUp };
        public static Buttons[] UtilityFButtons = { Buttons.DPadDown };

        // Jump
        public static Keys[] JumpKeys = { Keys.W };
        public static Buttons[] JumpButtons = { Buttons.A };

        // Attack
        public static Keys[] AttackKeys = { Keys.Space };
        public static Buttons[] AttackButtons = { Buttons.B };

        // Start Menu
        public static Keys[] MenuKeys = { Keys.Escape, Keys.Enter };
        public static Buttons[] MenuButtons = { Buttons.Start };

        // Debug
        public static Keys[] DebugKeys = { Keys.OemTilde };
        public static Buttons[] DebugButtons = { Buttons.RightStick };

        // Bumpers
        public static Keys[] LeftBumperKeys = { Keys.Z };
        public static Buttons[] LeftBumperButtons = { Buttons.LeftShoulder };
        public static Keys[] RightBumperKeys = { Keys.C };
        public static Buttons[] RightBumperButtons = { Buttons.RightShoulder };

        // Trigger keys
        public static Keys[] LeftTriggerKeys = { Keys.Q };
        public static Buttons[] LeftTriggerButtons = { };
        public static Keys[] RightTriggerKeys = { Keys.E };
        public static Buttons[] RightTriggerButtons = { };

        // Editor Saving
        public static Keys[] SaveKeys = { Keys.S };
        public static Buttons[] SaveButtons = { };

        // General Purpose
        public static Keys[] ControlKeys = { Keys.LeftControl, Keys.RightControl };
        public static Buttons[] ControlButtons = { };
        public static Keys[] ShiftKeys = { Keys.LeftShift, Keys.RightShift };
        public static Buttons[] ShiftButtons = { };
        public static Keys[] AltKeys = { Keys.LeftAlt, Keys.RightAlt };
        public static Buttons[] AltButtons = { };
        public static Keys[] TabKeys = { Keys.Tab };
        public static Buttons[] TabButtons = { };
        public static Keys[] CAPSKeys = { Keys.CapsLock };
        public static Buttons[] CAPSButtons = { };

        #region public methods

        public static void BeginUpdate(GameTime GameTime)
        {
            GamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            MouseCoords.X = MouseState.X;
            MouseCoords.Y = MouseState.Y;

            // Age time since inputs were pressed
            foreach (var Next in InputLookupTable.ToList())
            {
                if (Next.Value > 0.0f)
                {
                    PreparedInputLookupTable[Next.Key] -= GameTime.ElapsedGameTime.Milliseconds;
                }
            }
        }

        public static void EndUpdate()
        {
            LastGamePadState = GamePadState;
            LastKeyboardState = KeyboardState;
            LastMouseState = MouseState;

            foreach (var Next in InputLookupTable.ToList())
            {
                InputLookupTable[Next.Key] = PreparedInputLookupTable[Next.Key];
            }
        }

        // Cycle the input early to avoid multiple JustPressed() or JustReleased() processing
        // This is useful when exiting a menu if the exit button also does something in the game,
        // that way both do not get executed
        public static void JamInput()
        {
            EndUpdate();
        }

        public static void ClearPrimaryDirectionalDelay(InputDirectionEnum InputDirection)
        {
            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    ClearDelays(DummyLeftThumbStickKeysLeft, DummyLeftThumbStickButtonsLeft);
                    ClearDelays(LeftKeys, LeftButtons);
                    break;
                case InputDirectionEnum.Right:
                    ClearDelays(DummyLeftThumbStickKeysRight, DummyLeftThumbStickButtonsRight);
                    ClearDelays(RightKeys, RightButtons);
                    break;
                case InputDirectionEnum.Up:
                    ClearDelays(DummyLeftThumbStickKeysUp, DummyLeftThumbStickButtonsUp);
                    ClearDelays(UpKeys, UpButtons);
                    break;
                case InputDirectionEnum.Down:
                    ClearDelays(DummyLeftThumbStickKeysDown, DummyLeftThumbStickButtonsDown);
                    ClearDelays(DownKeys, DownButtons);
                    break;
            }
        }

        public static void ClearDelays(Keys[] KeyList, Buttons[] ButtonList)
        {
            CacheInput(KeyList, ButtonList, InputActionEnum.Down, 0.0f);
            CacheInput(KeyList, ButtonList, InputActionEnum.Pressed, 0.0f);
            CacheInput(KeyList, ButtonList, InputActionEnum.Released, 0.0f);
        }

        public static void ClearSecondaryDirectionalDelay(InputDirectionEnum InputDirection)
        {
            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    ClearDelays(DummyRightThumbStickKeysLeft, DummyRightThumbStickButtonsLeft);
                    ClearDelays(AlternativeLeftKeys, AlternativeLeftButtons);
                    break;
                case InputDirectionEnum.Right:
                    ClearDelays(DummyRightThumbStickKeysRight, DummyRightThumbStickButtonsRight);
                    ClearDelays(AlternativeRightKeys, AlternativeRightButtons);
                    break;
                case InputDirectionEnum.Up:
                    ClearDelays(DummyRightThumbStickKeysUp, DummyRightThumbStickButtonsUp);
                    ClearDelays(AlternativeUpKeys, AlternativeUpButtons);
                    break;
                case InputDirectionEnum.Down:
                    ClearDelays(DummyRightThumbStickKeysDown, DummyRightThumbStickButtonsDown);
                    ClearDelays(AlternativeDownKeys, AlternativeDownButtons);
                    break;
            }
        }

        /// <summary>
        /// Determines if the mouse just moved
        /// </summary>
        public static bool CheckMouseJustMoved()
        {
            if (LastMouseState.X != MouseState.X || LastMouseState.Y != MouseState.Y)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the mouse was just clicked
        /// </summary>
        public static bool CheckMouseJustClicked()
        {
            if (LastMouseState.LeftButton == ButtonState.Released && MouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the mouse was just released
        /// </summary>
        public static bool CheckMouseJustReleased()
        {
            if (LastMouseState.LeftButton == ButtonState.Pressed && MouseState.LeftButton == ButtonState.Released)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if any of the given input buttons were just pressed
        /// </summary>
        public static bool CheckJustPressed(Keys[] KeyList, Buttons[] ButtonList, float Delay = StandardInputDelay)
        {
            if (!InputReady(KeyList, ButtonList, InputActionEnum.Pressed))
                return false;

            // Check if a key was just pressed
            if (CheckKeysUp(KeyList, InputStateEnum.LastState) && CheckKeysDown(KeyList, InputStateEnum.CurrentState))
            {
                CacheInput(KeyList, ButtonList, InputActionEnum.Pressed, Delay);
                return true;
            }

            // Check if a button was just pressed
            if (CheckButtonsUp(ButtonList, InputStateEnum.LastState) && CheckButtonsDown(ButtonList, InputStateEnum.CurrentState))
            {
                CacheInput(KeyList, ButtonList, InputActionEnum.Pressed, Delay);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if any of the given input buttons were just released
        /// </summary>
        public static bool CheckJustReleased(Keys[] KeyList, Buttons[] ButtonList, float Delay = StandardInputDelay)
        {
            if (!InputReady(KeyList, ButtonList, InputActionEnum.Released))
                return false;

            // Check if a key was just released
            if (CheckKeysDown(KeyList, InputStateEnum.LastState) && CheckKeysUp(KeyList, InputStateEnum.CurrentState))
            {
                CacheInput(KeyList, ButtonList, InputActionEnum.Released, Delay);
                return true;
            }

            // Check if a button was just released
            if (CheckButtonsDown(ButtonList, InputStateEnum.LastState) && CheckButtonsUp(ButtonList, InputStateEnum.CurrentState))
            {
                CacheInput(KeyList, ButtonList, InputActionEnum.Released, Delay);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if any of the given input is being held down
        /// </summary>
        public static bool CheckInputDown(Keys[] KeyList, Buttons[] ButtonList, float Delay = StandardInputDelay)
        {
            if (!InputReady(KeyList, ButtonList, InputActionEnum.Down))
                return false;

            // Check if a key was just pressed
            if (CheckKeysDown(KeyList, InputStateEnum.CurrentState))
            {
                CacheInput(KeyList, ButtonList, InputActionEnum.Down, Delay);
                return true;
            }

            // Check if a button was just pressed
            if (CheckButtonsDown(ButtonList, InputStateEnum.CurrentState))
            {
                CacheInput(KeyList, ButtonList, InputActionEnum.Down, Delay);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if directional input was just engaged
        /// </summary>
        public static bool CheckPrimaryDirectionJustPressed(InputDirectionEnum InputDirection, float Delay = StandardInputDelay, float ThumbStickThreshold = ThumbStickPressThreshold)
        {
            float Dummy;
            return (CheckPrimaryDirectionJustPressed(InputDirection, ThumbStickThreshold, Delay, out Dummy));
        }
        public static bool CheckPrimaryDirectionJustPressed(InputDirectionEnum InputDirection, float ThumbStickThreshold, float Delay, out float ThumbStickValue)
        {
            ThumbStickValue = 0.0f;

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (!InputReady(DummyLeftThumbStickKeysLeft, DummyLeftThumbStickButtonsLeft, InputActionEnum.Pressed))
                        return false;
                    break;
                case InputDirectionEnum.Right:
                    if (!InputReady(DummyLeftThumbStickKeysRight, DummyLeftThumbStickButtonsRight, InputActionEnum.Pressed))
                        return false;
                    break;
                case InputDirectionEnum.Up:
                    if (!InputReady(DummyLeftThumbStickKeysUp, DummyLeftThumbStickButtonsUp, InputActionEnum.Pressed))
                        return false;
                    break;
                case InputDirectionEnum.Down:
                    if (!InputReady(DummyLeftThumbStickKeysDown, DummyLeftThumbStickButtonsDown, InputActionEnum.Pressed))
                        return false;
                    break;
            }

            if (CheckLeftThumbStickJustPressed(InputDirection, ThumbStickThreshold, out ThumbStickValue))
            {
                switch (InputDirection)
                {
                    case InputDirectionEnum.None:
                        break;
                    case InputDirectionEnum.Left:
                        CacheInput(DummyLeftThumbStickKeysLeft, DummyLeftThumbStickButtonsLeft, InputActionEnum.Pressed, Delay);
                        break;
                    case InputDirectionEnum.Right:
                        CacheInput(DummyLeftThumbStickKeysRight, DummyLeftThumbStickButtonsRight, InputActionEnum.Pressed, Delay);
                        break;
                    case InputDirectionEnum.Up:
                        CacheInput(DummyLeftThumbStickKeysUp, DummyLeftThumbStickButtonsUp, InputActionEnum.Pressed, Delay);
                        break;
                    case InputDirectionEnum.Down:
                        CacheInput(DummyLeftThumbStickKeysDown, DummyLeftThumbStickButtonsDown, InputActionEnum.Pressed, Delay);
                        break;
                }
                return true;
            }

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    ThumbStickValue = -1.0f;
                    if (CheckJustPressed(LeftKeys, LeftButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Right:
                    ThumbStickValue = 1.0f;
                    if (CheckJustPressed(RightKeys, RightButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Up:
                    ThumbStickValue = 1.0f;
                    if (CheckJustPressed(UpKeys, UpButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Down:
                    ThumbStickValue = -1.0f;
                    if (CheckJustPressed(DownKeys, DownButtons, Delay))
                        return true;
                    break;
            }

            ThumbStickValue = 0.0f;

            return false;
        }

        /// <summary>
        /// Determines if directional input was just released
        /// </summary>
        public static bool CheckPrimaryDirectionJustReleased(InputDirectionEnum InputDirection, float Delay = StandardInputDelay, float ThumbStickThreshold = ThumbStickPressThreshold)
        {
            float Dummy;
            return (CheckPrimaryDirectionJustReleased(InputDirection, ThumbStickThreshold, Delay, out Dummy));
        }
        public static bool CheckPrimaryDirectionJustReleased(InputDirectionEnum InputDirection, float ThumbStickThreshold, float Delay, out float ThumbStickValue)
        {
            ThumbStickValue = 0.0f;

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (!InputReady(DummyLeftThumbStickKeysLeft, DummyLeftThumbStickButtonsLeft, InputActionEnum.Released))
                        return false;
                    break;
                case InputDirectionEnum.Right:
                    if (!InputReady(DummyLeftThumbStickKeysRight, DummyLeftThumbStickButtonsRight, InputActionEnum.Released))
                        return false;
                    break;
                case InputDirectionEnum.Up:
                    if (!InputReady(DummyLeftThumbStickKeysUp, DummyLeftThumbStickButtonsUp, InputActionEnum.Released))
                        return false;
                    break;
                case InputDirectionEnum.Down:
                    if (!InputReady(DummyLeftThumbStickKeysDown, DummyLeftThumbStickButtonsDown, InputActionEnum.Released))
                        return false;
                    break;
            }

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (CheckJustReleased(LeftKeys, LeftButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Right:
                    if (CheckJustReleased(RightKeys, RightButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Up:
                    if (CheckJustReleased(UpKeys, UpButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Down:
                    if (CheckJustReleased(DownKeys, DownButtons, Delay))
                        return true;
                    break;
            }

            if (CheckLeftThumbStickJustReleased(InputDirection, ThumbStickReleaseThreshold, out ThumbStickValue))
            {
                switch (InputDirection)
                {
                    case InputDirectionEnum.None:
                        break;
                    case InputDirectionEnum.Left:
                        CacheInput(DummyLeftThumbStickKeysLeft, DummyLeftThumbStickButtonsLeft, InputActionEnum.Released, Delay);
                        break;
                    case InputDirectionEnum.Right:
                        CacheInput(DummyLeftThumbStickKeysRight, DummyLeftThumbStickButtonsRight, InputActionEnum.Released, Delay);
                        break;
                    case InputDirectionEnum.Up:
                        CacheInput(DummyLeftThumbStickKeysUp, DummyLeftThumbStickButtonsUp, InputActionEnum.Released, Delay);
                        break;
                    case InputDirectionEnum.Down:
                        CacheInput(DummyLeftThumbStickKeysDown, DummyLeftThumbStickButtonsDown, InputActionEnum.Released, Delay);
                        break;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if directional input is held down
        /// </summary>
        public static bool CheckPrimaryDirectionDown(InputDirectionEnum InputDirection, float Delay = StandardInputDelay, float ThumbStickThreshold = ThumbStickPressThreshold)
        {
            float Dummy;
            return (CheckPrimaryDirectionDown(InputDirection, ThumbStickThreshold, Delay, out Dummy));
        }
        public static bool CheckPrimaryDirectionDown(InputDirectionEnum InputDirection, float ThumbStickThreshold, float Delay, out float ThumbStickValue)
        {
            ThumbStickValue = 0.0f;

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (!InputReady(DummyLeftThumbStickKeysLeft, DummyLeftThumbStickButtonsLeft, InputActionEnum.Down))
                        return false;
                    break;
                case InputDirectionEnum.Right:
                    if (!InputReady(DummyLeftThumbStickKeysRight, DummyLeftThumbStickButtonsRight, InputActionEnum.Down))
                        return false;
                    break;
                case InputDirectionEnum.Up:
                    if (!InputReady(DummyLeftThumbStickKeysUp, DummyLeftThumbStickButtonsUp, InputActionEnum.Down))
                        return false;
                    break;
                case InputDirectionEnum.Down:
                    if (!InputReady(DummyLeftThumbStickKeysDown, DummyLeftThumbStickButtonsDown, InputActionEnum.Down))
                        return false;
                    break;
            }

            if (CheckLeftThumbStickEngaged(InputDirection, InputStateEnum.CurrentState, ThumbStickReleaseThreshold, out ThumbStickValue))
            {
                switch (InputDirection)
                {
                    case InputDirectionEnum.None:
                        break;
                    case InputDirectionEnum.Left:
                        CacheInput(DummyLeftThumbStickKeysLeft, DummyLeftThumbStickButtonsLeft, InputActionEnum.Down, Delay);
                        break;
                    case InputDirectionEnum.Right:
                        CacheInput(DummyLeftThumbStickKeysRight, DummyLeftThumbStickButtonsRight, InputActionEnum.Down, Delay);
                        break;
                    case InputDirectionEnum.Up:
                        CacheInput(DummyLeftThumbStickKeysUp, DummyLeftThumbStickButtonsUp, InputActionEnum.Down, Delay);
                        break;
                    case InputDirectionEnum.Down:
                        CacheInput(DummyLeftThumbStickKeysDown, DummyLeftThumbStickButtonsDown, InputActionEnum.Down, Delay);
                        break;
                }
                return true;
            }

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    ThumbStickValue = -1.0f;
                    if (CheckInputDown(LeftKeys, LeftButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Right:
                    ThumbStickValue = 1.0f;
                    if (CheckInputDown(RightKeys, RightButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Up:
                    ThumbStickValue = 1.0f;
                    if (CheckInputDown(UpKeys, UpButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Down:
                    ThumbStickValue = -1.0f;
                    if (CheckInputDown(DownKeys, DownButtons, Delay))
                        return true;
                    break;
            }

            ThumbStickValue = 0.0f;

            return false;
        }

        /// <summary>
        /// Determines if directional input was just engaged
        /// </summary>
        public static bool CheckSecondaryDirectionJustPressed(InputDirectionEnum InputDirection, float Delay = StandardInputDelay, float ThumbStickThreshold = ThumbStickPressThreshold)
        {
            float Dummy;
            return (CheckSecondaryDirectionJustPressed(InputDirection, ThumbStickThreshold, Delay, out Dummy));
        }
        public static bool CheckSecondaryDirectionJustPressed(InputDirectionEnum InputDirection, float ThumbStickThreshold, float Delay, out float ThumbStickValue)
        {
            ThumbStickValue = 0.0f;

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (!InputReady(DummyRightThumbStickKeysLeft, DummyRightThumbStickButtonsLeft, InputActionEnum.Pressed))
                        return false;
                    break;
                case InputDirectionEnum.Right:
                    if (!InputReady(DummyRightThumbStickKeysRight, DummyRightThumbStickButtonsRight, InputActionEnum.Pressed))
                        return false;
                    break;
                case InputDirectionEnum.Up:
                    if (!InputReady(DummyRightThumbStickKeysUp, DummyRightThumbStickButtonsUp, InputActionEnum.Pressed))
                        return false;
                    break;
                case InputDirectionEnum.Down:
                    if (!InputReady(DummyRightThumbStickKeysDown, DummyRightThumbStickButtonsDown, InputActionEnum.Pressed))
                        return false;
                    break;
            }

            if (CheckRightThumbStickJustPressed(InputDirection, ThumbStickThreshold, out ThumbStickValue))
            {
                switch (InputDirection)
                {
                    case InputDirectionEnum.None:
                        break;
                    case InputDirectionEnum.Left:
                        CacheInput(DummyRightThumbStickKeysLeft, DummyRightThumbStickButtonsLeft, InputActionEnum.Pressed, Delay);
                        break;
                    case InputDirectionEnum.Right:
                        CacheInput(DummyRightThumbStickKeysRight, DummyRightThumbStickButtonsRight, InputActionEnum.Pressed, Delay);
                        break;
                    case InputDirectionEnum.Up:
                        CacheInput(DummyRightThumbStickKeysUp, DummyRightThumbStickButtonsUp, InputActionEnum.Pressed, Delay);
                        break;
                    case InputDirectionEnum.Down:
                        CacheInput(DummyRightThumbStickKeysDown, DummyRightThumbStickButtonsDown, InputActionEnum.Pressed, Delay);
                        break;
                }
                return true;
            }

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    ThumbStickValue = -1.0f;
                    if (CheckJustPressed(AlternativeLeftKeys, AlternativeLeftButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Right:
                    ThumbStickValue = 1.0f;
                    if (CheckJustPressed(AlternativeRightKeys, AlternativeRightButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Up:
                    ThumbStickValue = 1.0f;
                    if (CheckJustPressed(AlternativeUpKeys, AlternativeUpButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Down:
                    ThumbStickValue = -1.0f;
                    if (CheckJustPressed(AlternativeDownKeys, AlternativeDownButtons, Delay))
                        return true;
                    break;
            }

            ThumbStickValue = 0.0f;

            return false;
        }

        /// <summary>
        /// Determines if directional input was just engaged
        /// </summary>
        public static bool CheckSecondaryDirectionJustReleased(InputDirectionEnum InputDirection, float Delay = StandardInputDelay, float ThumbStickThreshold = ThumbStickPressThreshold)
        {
            float Dummy;
            return (CheckSecondaryDirectionJustReleased(InputDirection, ThumbStickThreshold, Delay, out Dummy));
        }
        public static bool CheckSecondaryDirectionJustReleased(InputDirectionEnum InputDirection, float ThumbStickThreshold, float Delay, out float ThumbStickValue)
        {
            ThumbStickValue = 0.0f;

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (!InputReady(DummyRightThumbStickKeysLeft, DummyRightThumbStickButtonsLeft, InputActionEnum.Released))
                        return false;
                    break;
                case InputDirectionEnum.Right:
                    if (!InputReady(DummyRightThumbStickKeysRight, DummyRightThumbStickButtonsRight, InputActionEnum.Released))
                        return false;
                    break;
                case InputDirectionEnum.Up:
                    if (!InputReady(DummyRightThumbStickKeysUp, DummyRightThumbStickButtonsUp, InputActionEnum.Released))
                        return false;
                    break;
                case InputDirectionEnum.Down:
                    if (!InputReady(DummyRightThumbStickKeysDown, DummyRightThumbStickButtonsDown, InputActionEnum.Released))
                        return false;
                    break;
            }

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (CheckJustReleased(AlternativeLeftKeys, AlternativeLeftButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Right:
                    if (CheckJustReleased(AlternativeRightKeys, AlternativeRightButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Up:
                    if (CheckJustReleased(AlternativeUpKeys, AlternativeUpButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Down:
                    if (CheckJustReleased(AlternativeDownKeys, AlternativeDownButtons, Delay))
                        return true;
                    break;
            }

            if (CheckRightThumbStickJustReleased(InputDirection, ThumbStickReleaseThreshold, out ThumbStickValue))
            {
                switch (InputDirection)
                {
                    case InputDirectionEnum.None:
                        break;
                    case InputDirectionEnum.Left:
                        CacheInput(DummyRightThumbStickKeysLeft, DummyRightThumbStickButtonsLeft, InputActionEnum.Released, Delay);
                        break;
                    case InputDirectionEnum.Right:
                        CacheInput(DummyRightThumbStickKeysRight, DummyRightThumbStickButtonsRight, InputActionEnum.Released, Delay);
                        break;
                    case InputDirectionEnum.Up:
                        CacheInput(DummyRightThumbStickKeysUp, DummyRightThumbStickButtonsUp, InputActionEnum.Released, Delay);
                        break;
                    case InputDirectionEnum.Down:
                        CacheInput(DummyRightThumbStickKeysDown, DummyRightThumbStickButtonsDown, InputActionEnum.Released, Delay);
                        break;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if directional input was just engaged
        /// </summary>
        public static bool CheckSecondaryDirectionDown(InputDirectionEnum InputDirection, float Delay = StandardInputDelay, float ThumbStickThreshold = ThumbStickPressThreshold)
        {
            float Dummy;
            return (CheckSecondaryDirectionPressed(InputDirection, ThumbStickThreshold, Delay, out Dummy));
        }
        public static bool CheckSecondaryDirectionPressed(InputDirectionEnum InputDirection, float ThumbStickThreshold, float Delay, out float ThumbStickValue)
        {
            ThumbStickValue = 0.0f;

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (!InputReady(DummyRightThumbStickKeysLeft, DummyRightThumbStickButtonsLeft, InputActionEnum.Down))
                        return false;
                    break;
                case InputDirectionEnum.Right:
                    if (!InputReady(DummyRightThumbStickKeysRight, DummyRightThumbStickButtonsRight, InputActionEnum.Down))
                        return false;
                    break;
                case InputDirectionEnum.Up:
                    if (!InputReady(DummyRightThumbStickKeysUp, DummyRightThumbStickButtonsUp, InputActionEnum.Down))
                        return false;
                    break;
                case InputDirectionEnum.Down:
                    if (!InputReady(DummyRightThumbStickKeysDown, DummyRightThumbStickButtonsDown, InputActionEnum.Down))
                        return false;
                    break;
            }

            if (CheckRightThumbStickEngaged(InputDirection, InputStateEnum.CurrentState, ThumbStickReleaseThreshold, out ThumbStickValue))
            {
                switch (InputDirection)
                {
                    case InputDirectionEnum.None:
                        break;
                    case InputDirectionEnum.Left:
                        CacheInput(DummyRightThumbStickKeysLeft, DummyRightThumbStickButtonsLeft, InputActionEnum.Down, Delay);
                        break;
                    case InputDirectionEnum.Right:
                        CacheInput(DummyRightThumbStickKeysRight, DummyRightThumbStickButtonsRight, InputActionEnum.Down, Delay);
                        break;
                    case InputDirectionEnum.Up:
                        CacheInput(DummyRightThumbStickKeysUp, DummyRightThumbStickButtonsUp, InputActionEnum.Down, Delay);
                        break;
                    case InputDirectionEnum.Down:
                        CacheInput(DummyRightThumbStickKeysDown, DummyRightThumbStickButtonsDown, InputActionEnum.Down, Delay);
                        break;
                }
                return true;
            }

            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    ThumbStickValue = -1.0f;
                    if (CheckInputDown(AlternativeLeftKeys, AlternativeLeftButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Right:
                    ThumbStickValue = 1.0f;
                    if (CheckInputDown(AlternativeRightKeys, AlternativeRightButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Up:
                    ThumbStickValue = 1.0f;
                    if (CheckInputDown(AlternativeUpKeys, AlternativeUpButtons, Delay))
                        return true;
                    break;
                case InputDirectionEnum.Down:
                    ThumbStickValue = -1.0f;
                    if (CheckInputDown(AlternativeDownKeys, AlternativeDownButtons, Delay))
                        return true;
                    break;
            }

            ThumbStickValue = 0.0f;

            return false;
        }

        // Determines if the right trigger was just released
        public static bool CheckRightTriggerJustReleased(float TriggerThreshold = TriggerReleaseThreshold, float Delay = StandardInputDelay)
        {
            float Dummy;
            return (CheckRightTriggerJustReleased(TriggerReleaseThreshold, Delay, out Dummy));
        }
        public static bool CheckRightTriggerJustReleased(float TriggerThreshold, float Delay, out float TriggerValue)
        {
            TriggerValue = 0.0f;

            if (!InputReady(DummyRightTriggerKeys, DummyRightTriggerButtons, InputActionEnum.Released))
                return false;

            if (CheckJustReleased(RightTriggerKeys, RightTriggerButtons, Delay))
                return true;

            if (CheckRightTriggerEngaged(InputStateEnum.LastState, TriggerThreshold, out TriggerValue) &&
                CheckRightTriggerDisengaged(InputStateEnum.CurrentState, TriggerThreshold, out TriggerValue))
            {
                CacheInput(DummyRightTriggerKeys, DummyRightTriggerButtons, InputActionEnum.Released, Delay);
                return true;
            }
            return false;
        }

        // Determines if the right trigger was just engaged
        public static bool CheckRightTriggerJustPressed(float TriggerThreshold = TriggerPressThreshold, float Delay = StandardInputDelay)
        {
            float Dummy;
            return (CheckRightTriggerJustPressed(TriggerPressThreshold, Delay, out Dummy));
        }
        public static bool CheckRightTriggerJustPressed(float TriggerThreshold, float Delay, out float TriggerValue)
        {
            TriggerValue = 0.0f;

            if (!InputReady(DummyRightTriggerKeys, DummyRightTriggerButtons, InputActionEnum.Pressed))
                return false;

            if (CheckRightTriggerDisengaged(InputStateEnum.LastState, TriggerThreshold, out TriggerValue) &&
                CheckRightTriggerEngaged(InputStateEnum.CurrentState, TriggerThreshold, out TriggerValue))
            {
                CacheInput(DummyRightTriggerKeys, DummyRightTriggerButtons, InputActionEnum.Pressed, Delay);
                return true;
            }

            TriggerValue = 1.0f;
            if (CheckJustPressed(RightTriggerKeys, RightTriggerButtons))
                return true;

            TriggerValue = 0.0f;

            return false;
        }

        /// <summary>
        /// Determines if right trigger is held down
        /// </summary>
        public static bool CheckRightTriggerDown(float TriggerThreshold = TriggerPressThreshold, float Delay = StandardInputDelay)
        {
            float Dummy;
            return (CheckRightTriggerDown(TriggerThreshold, Delay, out Dummy));
        }
        public static bool CheckRightTriggerDown(float TriggerThreshold, float Delay, out float TriggerValue)
        {
            TriggerValue = 0.0f;

            if (!InputReady(DummyRightTriggerKeys, DummyRightTriggerButtons, InputActionEnum.Down))
                return false;

            if (CheckRightTriggerEngaged(InputStateEnum.CurrentState, TriggerThreshold, out TriggerValue))
            {
                CacheInput(DummyRightTriggerKeys, DummyRightTriggerButtons, InputActionEnum.Down, Delay);
                return true;
            }

            TriggerValue = 1.0f;
            if (CheckInputDown(RightTriggerKeys, RightTriggerButtons))
                return true;

            TriggerValue = 0.0f;

            return false;
        }

        // Determines if the left trigger was just released
        public static bool CheckLeftTriggerJustReleased(float TriggerThreshold = TriggerReleaseThreshold, float Delay = StandardInputDelay)
        {
            float Dummy;
            return (CheckLeftTriggerJustReleased(TriggerReleaseThreshold, Delay, out Dummy));
        }
        public static bool CheckLeftTriggerJustReleased(float TriggerThreshold, float Delay, out float TriggerValue)
        {
            TriggerValue = 0.0f;

            if (!InputReady(DummyLeftTriggerKeys, DummyLeftTriggerButtons, InputActionEnum.Released))
                return false;

            if (CheckJustReleased(LeftTriggerKeys, LeftTriggerButtons))
                return true;

            if (CheckLeftTriggerEngaged(InputStateEnum.LastState, TriggerThreshold, out TriggerValue) &&
                CheckLeftTriggerDisengaged(InputStateEnum.CurrentState, TriggerThreshold, out TriggerValue))
            {
                CacheInput(DummyLeftTriggerKeys, DummyLeftTriggerButtons, InputActionEnum.Released, Delay);
                return true;
            }

            return false;
        }

        // Determines if the left trigger was just engaged
        public static bool CheckLeftTriggerJustPressed(float TriggerThreshold = TriggerPressThreshold, float Delay = StandardInputDelay)
        {
            float Dummy;
            return (CheckLeftTriggerJustPressed(TriggerPressThreshold, Delay, out Dummy));
        }
        public static bool CheckLeftTriggerJustPressed(float TriggerThreshold, float Delay, out float TriggerValue)
        {
            TriggerValue = 0.0f;

            if (!InputReady(DummyLeftTriggerKeys, DummyLeftTriggerButtons, InputActionEnum.Pressed))
                return false;

            if (CheckLeftTriggerDisengaged(InputStateEnum.LastState, TriggerThreshold, out TriggerValue) &&
                CheckLeftTriggerEngaged(InputStateEnum.CurrentState, TriggerThreshold, out TriggerValue))
            {
                CacheInput(DummyLeftTriggerKeys, DummyLeftTriggerButtons, InputActionEnum.Pressed, Delay);
                return true;
            }

            TriggerValue = 1.0f;
            if (CheckJustPressed(LeftTriggerKeys, LeftTriggerButtons))
                return true;

            TriggerValue = 0.0f;

            return false;
        }

        /// <summary>
        /// Determines if left trigger is held down
        /// </summary>
        public static bool CheckLeftTriggerDown(float TriggerThreshold = TriggerPressThreshold, float Delay = StandardInputDelay)
        {
            float Dummy;
            return (CheckLeftTriggerDown(TriggerThreshold, Delay, out Dummy));
        }
        public static bool CheckLeftTriggerDown(float TriggerThreshold, float Delay, out float TriggerValue)
        {
            TriggerValue = 0.0f;

            if (!InputReady(DummyLeftTriggerKeys, DummyLeftTriggerButtons, InputActionEnum.Down))
                return false;

            if (CheckLeftTriggerEngaged(InputStateEnum.CurrentState, TriggerThreshold, out TriggerValue))
            {
                CacheInput(DummyLeftTriggerKeys, DummyLeftTriggerButtons, InputActionEnum.Down, Delay);
                return true;
            }

            TriggerValue = 1.0f;
            if (CheckInputDown(LeftTriggerKeys, LeftTriggerButtons))
                return true;

            TriggerValue = 0.0f;

            return false;
        }

        #endregion

        # region private methods

        private static void CacheInput(Keys[] KeyList, Buttons[] ButtonList, InputActionEnum InputAction, float Delay)
        {
            PressedInput.Keys = KeyList;
            PressedInput.Buttons = ButtonList;
            PressedInput.InputAction = InputAction;

            // Cache to the lookup table that will be used for the next update cycle
            PreparedInputLookupTable[PressedInput] = Delay;
        }

        private static bool InputReady(Keys[] KeyList, Buttons[] ButtonList, InputActionEnum InputAction)
        {
            PressedInput.Keys = KeyList;
            PressedInput.Buttons = ButtonList;
            PressedInput.InputAction = InputAction;

            if (!InputLookupTable.ContainsKey(PressedInput))
                InputLookupTable.Add(PressedInput, 0.0f);

            if (!PreparedInputLookupTable.ContainsKey(PressedInput))
                PreparedInputLookupTable.Add(PressedInput, 0.0f);

            // Check for elements that are not ready
            if (InputLookupTable[PressedInput] > 0.0f)
                return false;

            // Input is ready
            return true;
        }

        // Determines if the thumbstick was just released
        private static bool CheckRightThumbStickJustReleased(InputDirectionEnum InputDirection, float ThumbStickThreshold, out float ThumbStickValue)
        {
            // Check if directional input was just released
            if (CheckRightThumbStickEngaged(InputDirection, InputStateEnum.LastState, ThumbStickThreshold, out ThumbStickValue) &&
                CheckRightThumbStickDisengaged(InputDirection, InputStateEnum.CurrentState, ThumbStickThreshold, out ThumbStickValue))
            {
                return true;
            }
            return false;
        }

        // Determines if the thumbstick was just engaged
        private static bool CheckRightThumbStickJustPressed(InputDirectionEnum InputDirection, float ThumbStickThreshold, out float ThumbStickValue)
        {
            if (CheckRightThumbStickDisengaged(InputDirection, InputStateEnum.LastState, ThumbStickThreshold, out ThumbStickValue) &&
                CheckRightThumbStickEngaged(InputDirection, InputStateEnum.CurrentState, ThumbStickThreshold, out ThumbStickValue))
            {
                return true;
            }
            return false;
        }

        // Determines if the thumbstick is engaged based on the given threshold
        private static bool CheckRightThumbStickEngaged(InputDirectionEnum InputDirection, InputStateEnum InputState,
            float ThumbStickPressThreshold, out float ThumbStickValue)
        {
            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Right.X < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Right.X;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Right.X < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Right.X;
                            return true;
                        }
                    }
                    break;
                case InputDirectionEnum.Right:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Right.X > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Right.X;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Right.X > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Right.X;
                            return true;
                        }
                    }
                    break;
                case InputDirectionEnum.Up:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Right.Y > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Right.Y;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Right.Y > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Right.Y;
                            return true;
                        }
                    }
                    break;
                case InputDirectionEnum.Down:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Right.Y < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Right.Y;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Right.Y < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Right.Y;
                            return true;
                        }
                    }
                    break;
            }

            ThumbStickValue = 0.0f;
            return false;
        }

        // Determines if the thumbstick is not engaged based on the given threshold
        private static bool CheckRightThumbStickDisengaged(InputDirectionEnum InputDirection, InputStateEnum InputState,
            float ThumbStickPressThreshold, out float ThumbStickValue)
        {
            return (!CheckRightThumbStickEngaged(InputDirection, InputState, ThumbStickPressThreshold, out ThumbStickValue));
        }

        // Determines if the left thumbstick was just released
        private static bool CheckLeftThumbStickJustReleased(InputDirectionEnum InputDirection, float ThumbStickThreshold, out float ThumbStickValue)
        {
            // Check if directional input was just released
            if (CheckLeftThumbStickEngaged(InputDirection, InputStateEnum.LastState, ThumbStickThreshold, out ThumbStickValue) &&
                CheckLeftThumbStickDisengaged(InputDirection, InputStateEnum.CurrentState, ThumbStickThreshold, out ThumbStickValue))
            {
                return true;
            }
            return false;
        }

        // Determines if the left thumbstick was just engaged
        private static bool CheckLeftThumbStickJustPressed(InputDirectionEnum InputDirection, float ThumbStickThreshold, out float ThumbStickValue)
        {
            if (CheckLeftThumbStickDisengaged(InputDirection, InputStateEnum.LastState, ThumbStickThreshold, out ThumbStickValue) &&
                CheckLeftThumbStickEngaged(InputDirection, InputStateEnum.CurrentState, ThumbStickThreshold, out ThumbStickValue))
            {
                return true;
            }
            return false;
        }

        // Determines if the left thumbstick is engaged based on the given threshold
        private static bool CheckLeftThumbStickEngaged(InputDirectionEnum InputDirection, InputStateEnum InputState,
            float ThumbStickPressThreshold, out float ThumbStickValue)
        {
            switch (InputDirection)
            {
                case InputDirectionEnum.None:
                    break;
                case InputDirectionEnum.Left:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Left.X < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Left.X;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Left.X < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Left.X;
                            return true;
                        }
                    }
                    break;
                case InputDirectionEnum.Right:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Left.X > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Left.X;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Left.X > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Left.X;
                            return true;
                        }
                    }
                    break;
                case InputDirectionEnum.Up:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Left.Y > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Left.Y;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Left.Y > ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Left.Y;
                            return true;
                        }
                    }
                    break;
                case InputDirectionEnum.Down:
                    if (InputState == InputStateEnum.CurrentState)
                    {
                        if (GamePadState.ThumbSticks.Left.Y < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = GamePadState.ThumbSticks.Left.Y;
                            return true;
                        }
                    }
                    else
                    {
                        if (LastGamePadState.ThumbSticks.Left.Y < -ThumbStickPressThreshold)
                        {
                            ThumbStickValue = LastGamePadState.ThumbSticks.Left.Y;
                            return true;
                        }
                    }
                    break;
            }

            ThumbStickValue = 0.0f;
            return false;
        }

        // Determines if the left thumbstick is not engaged based on the given threshold
        private static bool CheckLeftThumbStickDisengaged(InputDirectionEnum InputDirection, InputStateEnum InputState,
            float ThumbStickPressThreshold, out float ThumbStickValue)
        {
            return (!CheckLeftThumbStickEngaged(InputDirection, InputState, ThumbStickPressThreshold, out ThumbStickValue));
        }

        // Determines if the right trigger is engaged based on the given threshold
        private static bool CheckRightTriggerEngaged(InputStateEnum InputState, float TriggerPressThreshold, out float TriggerValue)
        {

            if (InputState == InputStateEnum.CurrentState)
            {
                if (GamePadState.Triggers.Right > ThumbStickPressThreshold)
                {
                    TriggerValue = GamePadState.Triggers.Right;
                    return true;
                }
            }
            else
            {
                if (LastGamePadState.Triggers.Right > ThumbStickPressThreshold)
                {
                    TriggerValue = LastGamePadState.Triggers.Right;
                    return true;
                }
            }
            TriggerValue = 0.0f;
            return false;
        }

        // Determines if the right trigger is not engaged based on the given threshold
        private static bool CheckRightTriggerDisengaged(InputStateEnum InputState, float TriggerPressThreshold, out float TriggerValue)
        {
            return (!CheckRightTriggerEngaged(InputState, TriggerPressThreshold, out TriggerValue));
        }

        // Determines if the left trigger is engaged based on the given threshold
        private static bool CheckLeftTriggerEngaged(InputStateEnum InputState, float TriggerPressThreshold, out float TriggerValue)
        {

            if (InputState == InputStateEnum.CurrentState)
            {
                if (GamePadState.Triggers.Left > ThumbStickPressThreshold)
                {
                    TriggerValue = GamePadState.Triggers.Left;
                    return true;
                }
            }
            else
            {
                if (LastGamePadState.Triggers.Left > ThumbStickPressThreshold)
                {
                    TriggerValue = LastGamePadState.Triggers.Left;
                    return true;
                }
            }
            TriggerValue = 0.0f;
            return false;
        }

        // Determines if the left trigger is not engaged based on the given threshold
        private static bool CheckLeftTriggerDisengaged(InputStateEnum InputState, float TriggerPressThreshold, out float TriggerValue)
        {
            return (!CheckLeftTriggerEngaged(InputState, TriggerPressThreshold, out TriggerValue));
        }

        // Determines if any key in the given list is pressed
        private static bool CheckKeysDown(Keys[] KeyList, InputStateEnum InputState)
        {
            for (int i = 0; i < KeyList.Length; i++)
            {
                switch (InputState)
                {
                    case InputStateEnum.CurrentState:
                        if (KeyboardState.IsKeyDown(KeyList[i]))
                            return true;
                        break;
                    case InputStateEnum.LastState:
                        if (LastKeyboardState.IsKeyDown(KeyList[i]))
                            return true;
                        break;
                }
            }
            return false;
        }

        // Determines if no key in the given list is pressed
        private static bool CheckKeysUp(Keys[] KeyList, InputStateEnum InputState)
        {
            return (!CheckKeysDown(KeyList, InputState));
        }

        // Determines if any button in the given list is pressed
        private static bool CheckButtonsDown(Buttons[] ButtonList, InputStateEnum InputState)
        {
            for (int i = 0; i < ButtonList.Length; i++)
            {
                switch (InputState)
                {
                    case InputStateEnum.CurrentState:
                        if (GamePadState.IsButtonDown(ButtonList[i]))
                            return true;
                        break;
                    case InputStateEnum.LastState:
                        if (LastGamePadState.IsButtonDown(ButtonList[i]))
                            return true;
                        break;
                }
            }
            return false;
        }

        // Determines if no button in the given list is pressed
        private static bool CheckButtonsUp(Buttons[] ButtonList, InputStateEnum InputState)
        {
            return (!CheckButtonsDown(ButtonList, InputState));
        }

        #endregion

        enum InputActionEnum
        {
            Down,
            Pressed,
            Released,
        }

        struct InputPair
        {
            public Keys[] Keys;
            public Buttons[] Buttons;
            public InputActionEnum InputAction;

            public InputPair(Keys[] Keys, Buttons[] Buttons, InputActionEnum InputState)
            {
                this.Keys = Keys;
                this.Buttons = Buttons;
                this.InputAction = InputState;
            }
        }

    }

    enum InputStateEnum
    {
        LastState,
        CurrentState
    }

    enum InputDirectionEnum
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
}
