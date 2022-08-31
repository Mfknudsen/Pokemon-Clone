#region Packages

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.Systems
{
    public class InputManager
    {
        #region Values

        public static InputManager instance
        {
            get
            {
                _instance ??= new InputManager();

                return _instance;
            }
        }

        private static InputManager _instance;

        #region Events

        public readonly UnityEvent<Vector2>
            moveAxisInputEvent = new();

        public readonly UnityEvent
            nextChatInputEvent = new(),
            pauseInputEvent = new(),
            interactInputEvent = new(),
            showHideEvent = new(),
            rightClickEvent = new(),
            leftClickEvent = new();

        public readonly UnityEvent<bool>
            runInputEvent = new();

        #endregion

        #endregion

        #region Build In States

        private InputManager()
        {
            PlayerInput playerInput = new();

            playerInput.Player.Enable();

            playerInput.Player.MoveAxis.performed += context =>
            {
                Vector2 input = context.ReadValue<Vector2>();
                this.moveAxisInputEvent.Invoke(input);
            };
            playerInput.Player.MoveAxis.canceled += context =>
            {
                Vector2 input = context.ReadValue<Vector2>();
                this.moveAxisInputEvent.Invoke(input);
            };

            playerInput.Player.Run.performed += context => this.runInputEvent.Invoke(!context.canceled);
            playerInput.Player.Run.canceled += context => this.runInputEvent.Invoke(!context.canceled);

            playerInput.Player.NextChat.performed += _ => nextChatInputEvent.Invoke();
            playerInput.Player.Pause.performed += _ => pauseInputEvent.Invoke();
            playerInput.Player.Interact.performed += _ => interactInputEvent.Invoke();
            playerInput.Player.ShowHide.performed += _ => showHideEvent.Invoke();

            playerInput.Player.RightClick.performed += _ => rightClickEvent.Invoke();
            playerInput.Player.LeftClick.performed += _ => leftClickEvent.Invoke();
        }

        #endregion
    }
}