#region Packages

using Cinemachine;
using Runtime.Common.ScriptableVariables.Structs;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Player
{
    public class ItemThrower : MonoBehaviour
    {
        #region Values

        [SerializeField] private CinemachineVirtualCameraBase cameraRig;
        [SerializeField] private ScriptableBool aiming, throwing;

        #endregion

        #region Build In States

        private void OnEnable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.AddListener(() => this.aiming.value = !this.aiming.value);
            inputManager.leftClickEvent.AddListener(() => this.throwing.value = !this.throwing.value);
            
            aiming.AddListener(SwitchToAiming);
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.RemoveListener(() => this.aiming.value = !this.aiming.value);
            inputManager.leftClickEvent.RemoveListener(() => this.throwing.value = !this.throwing.value);
            
            aiming.RemoveListener(SwitchToAiming);
        }

        #endregion

        #region Internal

        private void SwitchToAiming(bool state)
        {
            if (state)
            {
                SwitchToAimingRig();
                SwitchToAimingControls();
            }
            else
            {
                
            }
        }

        private void SwitchToAimingRig()
        {
            cameraRig.enabled = true;
        }

        private void SwitchToAimingControls()
        {
            
        }

        #endregion
    }
}