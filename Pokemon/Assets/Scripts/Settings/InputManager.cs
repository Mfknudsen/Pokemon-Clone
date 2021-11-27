#region Packages

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Settings
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
    

    #endregion
    public class InputManager : MonoBehaviour, ISetup
    {
        #region Values

        public static InputManager instance;
        private PlayerInput playerInput;

        #region Events

        [HideInInspector] public MoveAxisInputEvent moveAxisInputEvent;
        [HideInInspector] public RotAxisInputEvent rotAxisInputEvent;

        #endregion

        #endregion

        #region Build In States

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        #endregion

        #region Getters

        public int Priority()
        {
            return 1;
        }

        #endregion

        #region In

        public void Setup()
        {
            playerInput = new PlayerInput();
            
            playerInput.Player.Enable();
            playerInput.Player.MoveAxis.performed += OnMoveAxisPerformed;
            playerInput.Player.TurnAxis.performed += OnRotAxisPerformed;
        }

        #endregion
        
        #region Internal

        private void OnMoveAxisPerformed(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            moveAxisInputEvent.Invoke(input);
            
            Debug.Log("move");
        }

        private void OnRotAxisPerformed(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            rotAxisInputEvent.Invoke(input);
        }

        #endregion
    }
}