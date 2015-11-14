using System.Linq;
using Microsoft.Xna.Framework.Input;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Input;

namespace Pyratron.UI.Monogame
{
    public class InputHandler : UI.InputHandler
    {
        public override Point MousePosition { get; protected set; }
        private float firstPress;
        private KeyboardState ks;
        private Keys lastKey;
        private Keys[] lastKeys;
        private KeyboardState lastks;
        private MouseState lastms;
        private MouseState ms;

        public InputHandler(UI.Manager manager) : base(manager)
        {
        }

        /// <summary>
        /// Checks if all of the keys specified are down
        /// </summary>
        public override bool AllKeysDown(params Key[] keys)
        {
            return keys.All(k => ks.IsKeyDown((Keys) k));
        }

        /// <summary>
        /// Checks if any of the keys specified are down
        /// </summary>
        public override bool AnyKeysDown(params Key[] keys)
        {
            return keys.Any(k => ks.IsKeyDown((Keys) k));
        }

        /// <summary>
        /// Checks if any of the keys specified are pressed/toggled
        /// </summary>
        public override bool AnyKeysPressed(params Key[] keys)
        {
            return keys.Any(k => ks.IsKeyUp((Keys) k) && lastks.IsKeyDown((Keys) k));
        }

        /// <summary>
        /// Checks if a given key is currently down
        /// </summary>
        public override bool IsKeyDown(Key key)
        {
            return ks.IsKeyDown((Keys) key);
        }

        /// <summary>
        /// Checks if a given key is currently being pressed (Was not pressed last state, but now is)
        /// </summary>
        public override bool IsKeyPressed(Key key)
        {
            return ks.IsKeyDown((Keys) key) && lastks.IsKeyUp((Keys) key);
        }

        /// <summary>
        /// Checks if a given key is currently up
        /// </summary>
        public override bool IsKeyUp(Key key)
        {
            return ks.IsKeyUp((Keys) key);
        }

        /// <summary>
        /// Checks if the left button is being clicked (Currently is down, wasn't last frame)
        /// </summary>
        /// <returns></returns>
        public override bool IsLeftClicked()
        {
            return ms.LeftButton == ButtonState.Pressed &&
                   lastms.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the left button is being held down
        /// </summary>
        public override bool IsLeftDown()
        {
            return ms.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the left button is currently up
        /// </summary>
        public override bool IsLeftUp()
        {
            return ms.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the right button is being clicked (Currently is down, wasn't last frame)
        /// </summary>
        /// <returns></returns>
        public override bool IsRightClicked()
        {
            return ms.RightButton == ButtonState.Pressed &&
                   lastms.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the right button is being held down
        /// </summary>
        public override bool IsRightDown()
        {
            return ms.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the right button is currently up
        /// </summary>
        public override bool IsRightUp()
        {
            return ms.RightButton == ButtonState.Released;
        }

        public override void Update(float elapsed, float total)
        {
            lastks = ks;
            lastms = ms;
            ms = Mouse.GetState();
            ks = Keyboard.GetState();
            var keys = ks.GetPressedKeys();

            // Mouse move
            if (lastms.Position != ms.Position)
                OnMouseMove(ms.Position.ToOriginal());

            // Mouse down
            if (ms.LeftButton == ButtonState.Pressed && lastms.LeftButton == ButtonState.Released)
                OnMouseDown(MouseButton.Left);
            if (ms.RightButton == ButtonState.Pressed && lastms.RightButton == ButtonState.Released)
                OnMouseDown(MouseButton.Right);
            if (ms.MiddleButton == ButtonState.Pressed && lastms.MiddleButton == ButtonState.Released)
                OnMouseDown(MouseButton.Middle);

            // Mouse up
            if (ms.LeftButton == ButtonState.Released && lastms.LeftButton == ButtonState.Pressed)
                OnMouseUp(MouseButton.Left);
            if (ms.RightButton == ButtonState.Released && lastms.RightButton == ButtonState.Pressed)
                OnMouseUp(MouseButton.Right);
            if (ms.MiddleButton == ButtonState.Released && lastms.MiddleButton == ButtonState.Pressed)
                OnMouseUp(MouseButton.Middle);

       
            if (lastKeys != null)
            {
                // Key down
                foreach (var key in keys.Where(key => !lastKeys.Contains(key)))
                {
                    OnKeyDown((Key) key);
                    firstPress = total;
                    lastKey = key;
                }
                // If a key is being held down, after .7 seconds, make it repeat.
                if (total > firstPress + .7f && IsKeyDown((Key) lastKey))
                {
                    OnKeyPress((Key) lastKey);
                    firstPress = total - .665f; // The next repeat time will be much shorter.
                }

                // Key up
                foreach (var key in lastKeys.Where(key => !keys.Contains(key)))
                {
                    OnKeyUp((Key) key);
                }
            }

            MousePosition = new Point(ms.Position.X, ms.Position.Y);

            lastKeys = keys;
        }

        /// <summary>
        /// Checks if all of the keys specified were up last frame
        /// </summary>
        public override bool WasAllKeysUp(params Key[] keys)
        {
            return keys.All(k => lastks.IsKeyUp((Keys) k));
        }

        /// <summary>
        /// Checks if any of the keys specified were down last frame
        /// </summary>
        public override bool WasAnyKeysDown(params Key[] keys)
        {
            return keys.Any(k => lastks.IsKeyDown((Keys) k));
        }

        /// <summary>
        /// Checks if a given key is was down last frame
        /// </summary>
        public override bool WasKeyDown(Key key)
        {
            return lastks.IsKeyDown((Keys) key);
        }

        /// <summary>
        /// Checks if a given key has been toggled (Was pressed last state, but now isn't)
        /// </summary>
        public override bool WasKeyPressed(Key key)
        {
            return ks.IsKeyUp((Keys) key) && lastks.IsKeyDown((Keys) key);
        }

        /// <summary>
        /// Checks if a given key is was up last frame
        /// </summary>
        public override bool WasKeyUp(Key key)
        {
            return lastks.IsKeyUp((Keys) key);
        }

        /// <summary>
        /// Checks if any of the keys specified were up the last frame
        /// </summary>
        public override bool WereAnyKeysUp(params Key[] keys)
        {
            return keys.Any(k => lastks.IsKeyUp((Keys) k));
        }
    }
}