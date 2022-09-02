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
                if (_instance != null) return _instance;

                Debug.Log("Instantiating new InputManager");
                _instance = new InputManager();

                return _instance;
            }
        }

        private static InputManager _instance;

        #region Events

        public readonly UnityEvent<Vector2>
            moveAxisInputEvent = new(),
            turnAxisInputEvent = new();

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
                Debug.Log(context.ReadValue<Vector2>());
                this.moveAxisInputEvent.Invoke(context.ReadValue<Vector2>());
            };
            playerInput.Player.MoveAxis.canceled += context =>
                this.moveAxisInputEvent.Invoke(context.ReadValue<Vector2>());

            playerInput.Player.TurnAxis.performed += context =>
                this.turnAxisInputEvent.Invoke(context.ReadValue<Vector2>());
            playerInput.Player.TurnAxis.canceled += context =>
                this.turnAxisInputEvent.Invoke(context.ReadValue<Vector2>());

            playerInput.Player.Run.performed += context => this.runInputEvent.Invoke(!context.canceled);
            playerInput.Player.Run.canceled += context => this.runInputEvent.Invoke(!context.canceled);

            playerInput.Player.NextChat.performed += _ => nextChatInputEvent.Invoke();
            playerInput.Player.Pause.performed += _ => pauseInputEvent.Invoke();
            playerInput.Player.Interact.performed += _ => interactInputEvent.Invoke();
            playerInput.Player.ShowHide.performed += _ => showHideEvent.Invoke();

            playerInput.Player.RightClick.performed += _ => rightClickEvent.Invoke();
            playerInput.Player.RightClick.canceled += _ => rightClickEvent.Invoke();

            playerInput.Player.LeftClick.performed += _ => leftClickEvent.Invoke();
            playerInput.Player.LeftClick.canceled += _ => leftClickEvent.Invoke();
        }

        #endregion
    }
}