#region Packages

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.Systems
{
    public class InputManager
    {
        #region Values

        public static InputManager instance => _instance ??= new InputManager();

        private static InputManager _instance;

        #region Events

        public readonly UnityEvent<Vector2>
            moveAxisInputEvent = new(),
            turnAxisInputEvent = new();

        public readonly UnityEvent
            nextChatInputEvent = new(),
            pauseInputEvent = new(),
            interactInputEvent = new(),
            showHideEvent = new();

        public readonly UnityEvent<bool>
            runInputEvent = new(),
            rightClickEvent = new(),
            leftClickEvent = new();

        #endregion

        #endregion

        #region Build In States

        private InputManager()
        {
            PlayerInput playerInput = new();

            playerInput.Player.Enable();

            playerInput.Player.MoveAxis.performed += context => this.moveAxisInputEvent.Invoke(context.ReadValue<Vector2>());
            playerInput.Player.MoveAxis.canceled += context => this.moveAxisInputEvent.Invoke(context.ReadValue<Vector2>());

            playerInput.Player.TurnAxis.performed += context => this.turnAxisInputEvent.Invoke(context.ReadValue<Vector2>());
            playerInput.Player.TurnAxis.canceled += context => this.turnAxisInputEvent.Invoke(context.ReadValue<Vector2>());

            playerInput.Player.Run.performed += _ => this.runInputEvent.Invoke(true);
            playerInput.Player.Run.canceled += _ => this.runInputEvent.Invoke(false);

            playerInput.Player.NextChat.performed += _ => this.nextChatInputEvent.Invoke();
            playerInput.Player.Pause.performed += _ => this.pauseInputEvent.Invoke();
            playerInput.Player.Interact.performed += _ => this.interactInputEvent.Invoke();
            playerInput.Player.ShowHide.performed += _ => this.showHideEvent.Invoke();

            playerInput.Player.RightClick.performed += _ => this.rightClickEvent.Invoke(true);
            playerInput.Player.RightClick.canceled += _ => this.rightClickEvent.Invoke(false);

            playerInput.Player.LeftClick.performed += _ => this.leftClickEvent.Invoke(true);
            playerInput.Player.LeftClick.canceled += _ => this.leftClickEvent.Invoke(false);
        }

        #endregion

        #region Internal

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void ResetInstance() => _instance = null;

        #endregion
    }
}