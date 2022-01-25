#region Packages

using System;
using System.Collections;
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

    [Serializable]
    public class RunInputEvent : UnityEvent<bool>
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
        [HideInInspector] public RunInputEvent runInputEvent;

        #endregion

        #endregion

        #region In

        public override IEnumerator Setup()
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
            playerInput.Player.MoveAxis.canceled += OnMoveAxisPerformed;

            playerInput.Player.TurnAxis.performed += OnRotAxisPerformed;
            playerInput.Player.TurnAxis.canceled += OnRotAxisPerformed;

            playerInput.Player.NextChat.performed += OnNextChatPerformed;
            playerInput.Player.Pause.performed += OnPausePerformed;
            playerInput.Player.Interact.performed += OnInteractPerformed;
            
            playerInput.Player.Run.performed += OnRunPerformed;
            playerInput.Player.Run.canceled += OnRunPerformed;
            
            yield break;
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
        
        #endregion

        #endregion
    }
}