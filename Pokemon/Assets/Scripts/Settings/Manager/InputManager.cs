#region Packages

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Settings.Manager
{
    public class InputManager
    {
        #region Values

        public static InputManager Instance
        {
            get
            {
                if (staticInstance == null)
                    staticInstance = new InputManager();

                return staticInstance;
            }
        }

        private static InputManager staticInstance;

        #region Events

        public UnityEvent<Vector2> 
            moveAxisInputEvent = new UnityEvent<Vector2>(),
            rotAxisInputEvent = new UnityEvent<Vector2>();

        public UnityEvent 
            nextChatInputEvent = new UnityEvent(),
            pauseInputEvent = new UnityEvent(),
            interactInputEvent = new UnityEvent(),
            showHideEvent = new UnityEvent();

        public UnityEvent<bool> 
            runInputEvent = new UnityEvent<bool>();

        #endregion

        #endregion

        #region Build In States

        public InputManager()
        {
            PlayerInput playerInput = new PlayerInput();

            playerInput.Player.Enable();

            playerInput.Player.MoveAxis.performed += OnMoveAxisPerformed;
            playerInput.Player.MoveAxis.canceled += OnMoveAxisPerformed;

            playerInput.Player.TurnAxis.performed += OnRotAxisPerformed;
            playerInput.Player.TurnAxis.canceled += OnRotAxisPerformed;

            playerInput.Player.NextChat.performed += OnNextChatPerformed;
            playerInput.Player.Pause.performed += OnPausePerformed;
            playerInput.Player.Interact.performed += OnInteractPerformed;

            playerInput.Player.Run.performed += OnRunPerformed;
            playerInput.Player.Run.canceled += OnRunPerformed;

            playerInput.Player.ShowHide.performed += OnShowHidePerformed;
        }

        #endregion

        #region Internal

        #region Axis

        private void OnMoveAxisPerformed(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            moveAxisInputEvent.Invoke(input);
        }

        private void OnRotAxisPerformed(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            rotAxisInputEvent.Invoke(input);
        }

        #endregion

        #region Buttons

        private void OnNextChatPerformed(InputAction.CallbackContext context)
        {
            nextChatInputEvent.Invoke();
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            pauseInputEvent.Invoke();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            interactInputEvent.Invoke();
        }

        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            runInputEvent.Invoke(!context.canceled);
        }

        private void OnShowHidePerformed(InputAction.CallbackContext context)
        {
            showHideEvent.Invoke();
        }

        #endregion

        #endregion
    }
}