#region Packages

using Cinemachine;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Player
{
    public class ItemThrower : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase cameraRig;

        private bool aim;

        private void Start()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.AddListener(OnRightClick);
            inputManager.leftClickEvent.AddListener(OnLeftClick);
        }

        private void OnDestroy()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.RemoveListener(OnRightClick);
            inputManager.leftClickEvent.RemoveListener(OnLeftClick);
        }

        #region Internal

        private void OnRightClick()
        {
            aim = !aim;
        }

        private void OnLeftClick()
        {
        }

        #endregion
    }
}