#region Packages

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Settings.Manager
{
    #region Events

    [Serializable]
    public class MoveAxisInputEvent : UnityEvent<Vector2>
    {
    }

    [Serializable]
    public class RotAxisInputEvent : UnityEvent<Vector2>
    {
    }

    [Serializable]
    public class NextChatInputEvent : UnityEvent
    {
    }

    [Serializable]
    public class PauseInputEvent : UnityEvent
    {
    }

    [Serializable]
    public class InteractInputEvent : UnityEvent
    {
    }

    #endregion

    public class InputManager : Manager
    {
        #region Values

        public static InputManager instance;
        private PlayerInput playerInput;

        #region Events

        [HideInInspector] public MoveAxisInputEvent moveAxisInputEvent;
        [HideInInspector] public RotAxisInputEvent rotAxisInputEvent;
        [HideInInspector] public NextChatInputEvent nextChatInputEvent;
        [HideInInspector] public PauseInputEvent pauseInputEvent;
        [HideInInspector] public InteractInputEvent interactInputEvent;

        #endregion

        #endregion

        #region In

        public override void Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
            
            playerInput = new PlayerInput();

            playerInput.Player.Enable();
            playerInput.Player.MoveAxis.performed += OnMoveAxisPerformed;
            playerInput.Player.TurnAxis.performed += OnRotAxisPerformed;
            playerInput.Player.NextChat.performed += OnNextChatPerformed;
            playerInput.Player.Pause.performed += OnPausePerformed;
            playerInput.Player.Interact.performed += OnInteractPerformed;
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
            if (!context.performed) return;

            nextChatInputEvent.Invoke();
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            pauseInputEvent.Invoke();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            interactInputEvent.Invoke();
        }

        #endregion

        #endregion
    }
}