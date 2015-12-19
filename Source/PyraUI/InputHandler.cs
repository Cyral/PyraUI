using System;
using System.Collections.Generic;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Input;

namespace Pyratron.UI
{
    /// <summary>
    /// Handle keyboard and mouse input.
    /// </summary>
    public abstract class InputHandler
    {
        // TODO: Support for other keyboards from different regions

        /// <summary>
        /// Keys that have an alternate value when shift is pressed.
        /// </summary>
        private static readonly Dictionary<Key, char> shiftChars = new Dictionary<Key, char>
        {
            {Key.D1, '!'},
            {Key.D2, '@'},
            {Key.D3, '#'},
            {Key.D4, '$'},
            {Key.D5, '%'},
            {Key.D6, '^'},
            {Key.D7, '&'},
            {Key.D8, '*'},
            {Key.D9, '('},
            {Key.D0, ')'},
            {Key.OemOpenBrackets, '{'},
            {Key.OemCloseBrackets, '}'},
            {Key.OemComma, '<'},
            {Key.OemPeriod, '>'},
            {Key.OemQuestion, '?'},
            {Key.OemQuotes, '"'},
            {Key.OemSemicolon, ':'},
            {Key.OemPlus, '+'},
            {Key.OemMinus, '_'},
            {Key.OemBackslash, '|'},
            {Key.OemTilde, '~'}
        };

        /// <summary>
        /// Characters to be added when a key is pressed.
        /// </summary>
        private static readonly Dictionary<Key, char> convertChars = new Dictionary<Key, char>
        {
            {Key.OemOpenBrackets, '['},
            {Key.OemCloseBrackets, ']'},
            {Key.OemBackslash, '\\'},
            {Key.OemComma, ','},
            {Key.OemPeriod, '.'},
            {Key.OemQuestion, '\\'},
            {Key.OemQuotes, '\''},
            {Key.OemSemicolon, ';'},
            {Key.OemPlus, '='},
            {Key.OemMinus, '-'},
            {Key.OemTilde, '`'},
            {Key.OemPipe, '|'},
        };

        /// <summary>
        /// Keys that should be ignored (not added to text, but handled elsewhere)
        /// </summary>
        private static readonly List<Key> ignoreKeys = new List<Key>
        {
            Key.LeftShift,
            Key.RightShift,
            Key.LeftAlt,
            Key.RightAlt,
            Key.LeftControl,
            Key.RightControl,
            Key.LeftWindows,
            Key.RightWindows,
            Key.CapsLock,
            Key.NumLock,
            Key.PageDown,
            Key.PageUp,
            Key.Home,
            Key.BrowserBack,
            Key.BrowserFavorites,
            Key.BrowserForward,
            Key.BrowserHome,
            Key.BrowserRefresh,
            Key.BrowserSearch,
            Key.BrowserStop,
            Key.F1,
            Key.F2,
            Key.F3,
            Key.F4,
            Key.F5,
            Key.F6,
            Key.F7,
            Key.F8,
            Key.F9,
            Key.F10,
            Key.F11,
            Key.F12,
            Key.F13,
            Key.F14,
            Key.F15,
            Key.F16,
            Key.F17,
            Key.F18,
            Key.F19,
            Key.F20,
            Key.F21,
            Key.F22,
            Key.F23,
            Key.F24,
            Key.PrintScreen,
            Key.Print,
            Key.Escape,
            Key.End,
            Key.Apps,
            Key.Attn,
            Key.MediaNextTrack,
            Key.MediaPlayPause,
            Key.MediaPreviousTrack,
            Key.MediaStop,
            Key.SelectMedia
        };

        public abstract Point MousePosition { get; protected set; }
        private readonly Manager manager;

        public InputHandler(Manager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Use the specified key to manipulate the text.
        /// </summary>
        public string AddKeyPress(string text, Key key)
        {
            if (ignoreKeys.Contains(key))
                return text;
            switch (key)
            {
                case Key.Back: // Backspace.
                    if (text.Length != 0)
                        text = text.Substring(0, text.Length - 1);
                    break;
                default:
                    if (convertChars.ContainsKey(key))
                    {
                        if ((manager.Input.IsShiftDown() || manager.Input.IsCapsLocked()) && shiftChars.ContainsKey(key))
                            text += shiftChars[key];

                        else
                            text += convertChars[key];
                    }
                    else
                    {
                        var c = (char) key; // Use ASCII value.
                        if (manager.Input.IsShiftDown() || manager.Input.IsCapsLocked())
                        {
                            // Handle certain upper case characters.
                            if (shiftChars.ContainsKey(key))
                                text += shiftChars[key];
                            else
                                text += char.ToUpper(c);
                        }
                        else
                            text += char.ToLower(c);
                    }
                    break;
            }
            return text;
        }

        /// <summary>
        /// Checks if all of the keys specified are down
        /// </summary>
        public abstract bool AllKeysDown(params Key[] keys);

        /// <summary>
        /// Checks if any of the keys specified are down
        /// </summary>
        public abstract bool AnyKeysDown(params Key[] keys);

        /// <summary>
        /// Checks if any of the keys specified are pressed/toggled
        /// </summary>
        public abstract bool AnyKeysPressed(params Key[] keys);

        public bool IsCapsLocked() => Console.CapsLock;

        /// <summary>
        /// Checks if a given key is currently down
        /// </summary>
        public abstract bool IsKeyDown(Key key);

        /// <summary>
        /// Checks if a given key is currently being pressed (Was not pressed last state, but now is)
        /// </summary>
        public abstract bool IsKeyPressed(Key key);

        /// <summary>
        /// Checks if a given key is currently up
        /// </summary>
        public abstract bool IsKeyUp(Key key);

        /// <summary>
        /// Checks if the left button is being clicked (Currently is down, wasn't last frame)
        /// </summary>
        /// <returns></returns>
        public abstract bool IsLeftClicked();

        /// <summary>
        /// Checks if the left button is being held down
        /// </summary>
        public abstract bool IsLeftDown();

        /// <summary>
        /// Checks if the left button is currently up
        /// </summary>
        public abstract bool IsLeftUp();

        /// <summary>
        /// Checks if the right button is being clicked (Currently is down, wasn't last frame)
        /// </summary>
        /// <returns></returns>
        public abstract bool IsRightClicked();

        /// <summary>
        /// Checks if the right button is being held down
        /// </summary>
        public abstract bool IsRightDown();

        /// <summary>
        /// Checks if the right button is currently up
        /// </summary>
        public abstract bool IsRightUp();

        public bool IsShiftDown() => AnyKeysDown(Key.LeftShift, Key.RightShift);

        /// <summary>
        /// When a key is pressed down.
        /// </summary>
        public event KeyDownEventHandler KeyDown;

        /// <summary>
        /// When a key is pressed, OR a certain amount of time has elapsed in which case it is "pressed" again. (If the user holds
        /// a key down).
        /// </summary>
        public event KeyDownEventHandler KeyPress;

        /// <summary>
        /// When a pressed key is released.
        /// </summary>
        public event KeyUpEventHandler KeyUp;

        /// <summary>
        /// When a mouse button is clicked down.
        /// </summary>
        public event MouseDownEventHandler MouseDown;

        /// <summary>
        /// When a mouse button is released.
        /// </summary>
        public event MouseMoveEventHandler MouseMove;

        /// <summary>
        /// When the mouse is moved.
        /// </summary>
        public event MouseUpEventHandler MouseUp;

        public void OnKeyDown(Key key)
        {
            KeyDown?.Invoke(key);
            KeyPress?.Invoke(key);
        }

        public void OnKeyPress(Key key) => KeyPress?.Invoke(key);
        public void OnKeyUp(Key key) => KeyUp?.Invoke(key);

        public void OnMouseDown(MouseButton button) => MouseDown?.Invoke(button);
        public void OnMouseMove(Point point) => MouseMove?.Invoke(point);
        public void OnMouseUp(MouseButton button) => MouseUp?.Invoke(button);

        public abstract void Update(float delta, float total);

        /// <summary>
        /// Checks if all of the keys specified were up last frame
        /// </summary>
        public abstract bool WasAllKeysUp(params Key[] keys);

        /// <summary>
        /// Checks if any of the keys specified were down last frame
        /// </summary>
        public abstract bool WasAnyKeysDown(params Key[] keys);

        /// <summary>
        /// Checks if a given key is was down last frame
        /// </summary>
        public abstract bool WasKeyDown(Key key);

        /// <summary>
        /// Checks if a given key has been toggled (Was pressed last state, but now isn't)
        /// </summary>
        public abstract bool WasKeyPressed(Key key);

        /// <summary>
        /// Checks if a given key is was up last frame
        /// </summary>
        public abstract bool WasKeyUp(Key key);

        /// <summary>
        /// Checks if any of the keys specified were up the last frame
        /// </summary>
        public abstract bool WereAnyKeysUp(params Key[] keys);

        public delegate void KeyDownEventHandler(Key key);

        public delegate void KeyPressEventHandler(Key key);

        public delegate void KeyUpEventHandler(Key key);

        public delegate void MouseDownEventHandler(MouseButton button);

        public delegate void MouseMoveEventHandler(Point point);

        public delegate void MouseUpEventHandler(MouseButton button);
    }
}