#region Packages

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Settings.Managers
{
    public class InputManager
    {
        #region Values

        public static InputManager Instance
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
            showHideEvent = new();

        public readonly UnityEvent<bool>
            runInputEvent = new();

        #endregion

        #endregion

        #region Build In States

        private InputManager()
        {
            PlayerInput playerInput = new();

            playerInput.Player.Enable();

            playerInput.Player.MoveAxis.performed += OnMoveAxisPerformed;
            playerInput.Player.MoveAxis.canceled += OnMoveAxisPerformed;

            playerInput.Player.Run.performed += OnRunPerformed;
            playerInput.Player.Run.canceled += OnRunPerformed;

            playerInput.Player.NextChat.performed += OnNextChatPerformed;
            playerInput.Player.Pause.performed += OnPausePerformed;
            playerInput.Player.Interact.performed += OnInteractPerformed;
            playerInput.Player.ShowHide.performed += OnShowHidePerformed;
        }

        #endregion

        #region Internal

        #region Axis

        private void OnMoveAxisPerformed(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            this.moveAxisInputEvent.Invoke(input);
        }

        #endregion

        #region Buttons

        private void OnNextChatPerformed(InputAction.CallbackContext context)
        {
            this.nextChatInputEvent.Invoke();
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            this.pauseInputEvent.Invoke();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            this.interactInputEvent.Invoke();
        }

        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            this.runInputEvent.Invoke(!context.canceled);
        }

        private void OnShowHidePerformed(InputAction.CallbackContext context)
        {
            this.showHideEvent.Invoke();
        }

        #endregion

        #endregion
    }
}