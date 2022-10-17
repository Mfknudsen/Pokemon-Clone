#region Packages

using Cinemachine;
using Runtime.Player.Camera;
using Runtime.Systems.PersistantRunner;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    [CreateAssetMenu(menuName = "Manager/Camera")]
    public class CameraManager : Manager, IFrameStart
    {
        #region Values

        [FoldoutGroup("Camera")] [SerializeField]
        private Camera currentCamera;

        [FoldoutGroup("Camera")] [SerializeField]
        private CinemachineFreeLook defaultCameraRig;

        private CinemachineVirtualCameraBase currentRig;
        private CameraSettings currentSettings;

        #endregion

        #region Build In States

        public void FrameStart()
        {
            this.currentSettings = CameraSettings.Default();
            
            this.currentRig = this.defaultCameraRig;
            this.defaultCameraRig.enabled = true;

            this.ready = true;
        }

        #endregion

        #region Getters

        public CinemachineFreeLook GetDefaultRig()
        {
            return this.defaultCameraRig;
        }

        public CinemachineVirtualCameraBase GetCurrentRig()
        {
            return this.currentRig;
        }

        public Camera GetCurrentCamera()
        {
            return this.currentCamera;
        }

        public CameraSettings GetCurrentSettings()
        {
            return this.currentSettings;
        }

        #endregion

        #region In

        public void SetCurrentRigToDefault()
        {
            if (this.currentRig != null) this.currentRig.enabled = false;
            this.defaultCameraRig.enabled = true;
            this.currentRig = this.defaultCameraRig;
        }

        public void SetCurrentRig(CinemachineVirtualCameraBase set, bool disablePrevious = false)
        {
            if (this.currentRig != null) this.currentRig.enabled = !disablePrevious;

            if (set != null)
                set.enabled = true;

            this.currentRig = set;
        }

        public void SetCameraSettings(CameraSettings cameraSettings)
        {
            this.currentSettings = cameraSettings;
            this.currentCamera.fieldOfView = cameraSettings.fov;
        }

        #endregion
    }
}