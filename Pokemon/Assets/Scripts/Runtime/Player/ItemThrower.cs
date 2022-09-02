#region Packages

using Cinemachine;
using Runtime.ScriptableVariables;
using Runtime.ScriptableVariables.Structs;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Player
{
    public class ItemThrower : MonoBehaviour
    {
        #region Values

        [SerializeField] private CinemachineVirtualCameraBase cameraRig;
        [SerializeField] private BoolVariable aiming, throwing, allowed;
        [SerializeField] private ItemVariable toThrow;

        #endregion

        #region Build In States

        private void OnEnable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.AddListener(() => this.aiming.value = !this.aiming.value);
            inputManager.leftClickEvent.AddListener(ThrowItem);

            aiming.AddListener(SwitchToAiming);
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.RemoveListener(() => this.aiming.value = !this.aiming.value);
            inputManager.leftClickEvent.RemoveListener(ThrowItem);

            aiming.RemoveListener(SwitchToAiming);
        }

        #endregion

        #region Internal

        private void ThrowItem()
        {
            if(this.toThrow.Empty()) return;

            this.throwing.value = true;
            
        }
        
        private void SwitchToAiming()
        {
            if (this.aiming.value)
            {
                this.cameraRig.Priority = 11;
            }
            else
            {
                this.cameraRig.Priority = 9;
            }
        }

        #endregion
    }
}