#region Packages

using Cinemachine;
using Runtime.Player.Camera;
using Runtime.ScriptableVariables;
using Runtime.ScriptableVariables.Objects.Cinemachine;
using Runtime.ScriptableVariables.Structs;
using Runtime.Systems;
using Sirenix.OdinInspector;
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
        [SerializeField] private CinemachineFreeLookVariable defaultOverworldRig;
        [SerializeField] private CinemachineBrainVariable cameraBrain;

        [HorizontalGroup("Rotation Speed")] [SerializeField]
        private float xSpeed, ySpeed;

        [SerializeField] private Vector2 localMinMax;

        #endregion

        private void OnDrawGizmos()
        {
            try
            {
                Transform playerTransform = transform.parent;
                Vector3 playerPos = playerTransform.position;
                Gizmos.DrawLine(playerPos, playerPos + (playerPos - cameraBrain.value.transform.position));
            }
            catch
            {
                // ignored
            }
        }

        #region Build In States

        private void OnEnable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.AddListener(input => this.aiming.value = input);
            inputManager.leftClickEvent.AddListener(_ => ThrowItem());

            aiming.AddListener(SwitchToAiming);
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.RemoveListener(input => this.aiming.value = input);
            inputManager.leftClickEvent.RemoveListener(_ => ThrowItem());

            aiming.RemoveListener(SwitchToAiming);
        }

        #endregion

        #region Internal

        private void ThrowItem()
        {
            if (this.toThrow.Empty()) return;

            this.throwing.value = true;
        }

        private void SwitchToAiming()
        {
            if (!this.allowed.value) return;

            CameraEvent cameraEvent;
            if (this.aiming.value)
            {
                cameraEvent = new CameraEvent(
                    this.cameraRig,
                    CameraSettings.Default(),
                    .5f,
                    .5f);
            }
            else
            {
                cameraEvent = new CameraEvent(
                    PlayerManager.instance.GetOverworldCameraRig(),
                    CameraSettings.Default(),
                    .5f,
                    .5f);
            }

            OperationManager.instance.AddAsyncOperationsContainer(new OperationsContainer(cameraEvent));
        }

        #endregion
    }
}