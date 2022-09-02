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
                if (_instance?.playerInput == null)
                    _instance = new InputManager();

                return _instance;
            }
        }

        private readonly PlayerInput playerInput;

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
            playerInput = new PlayerInput();

            playerInput.Player.Enable();

            playerInput.Player.MoveAxis.performed += context =>
                moveAxisInputEvent.Invoke(context.ReadValue<Vector2>());
            playerInput.Player.MoveAxis.canceled += context =>
                moveAxisInputEvent.Invoke(context.ReadValue<Vector2>());

            playerInput.Player.TurnAxis.performed += context =>
                turnAxisInputEvent.Invoke(context.ReadValue<Vector2>());
            playerInput.Player.TurnAxis.canceled += context =>
                turnAxisInputEvent.Invoke(context.ReadValue<Vector2>());

            playerInput.Player.Run.performed += context => runInputEvent.Invoke(!context.canceled);
            playerInput.Player.Run.canceled += context => runInputEvent.Invoke(!context.canceled);

            playerInput.Player.NextChat.performed += _ => nextChatInputEvent.Invoke();
            playerInput.Player.Pause.performed += _ => pauseInputEvent.Invoke();
            playerInput.Player.Interact.performed += _ => interactInputEvent.Invoke();
            playerInput.Player.ShowHide.performed += _ => showHideEvent.Invoke();

            playerInput.Player.RightClick.performed += _ => rightClickEvent.Invoke();
            playerInput.Player.RightClick.canceled += _ => rightClickEvent.Invoke();

            playerInput.Player.LeftClick.performed += _ => leftClickEvent.Invoke();
            playerInput.Player.LeftClick.canceled += _ => leftClickEvent.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void RemoveQuit() => _instance = null;

        #endregion
    }
}