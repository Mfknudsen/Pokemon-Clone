#region Packages

using System.Collections;
using Cinemachine;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Player.Camera
{
    [CreateAssetMenu(menuName = "Manager/Camera")]
    public class CameraManager : Manager
    {
        #region Values

        [FoldoutGroup("Camera")] [SerializeField]
        private UnityEngine.Camera currentCamera;

        [FoldoutGroup("Camera")] [SerializeField]
        private CinemachineFreeLook defaultCameraRig;

        private CinemachineVirtualCameraBase currentRig;
        private CameraSettings currentSettings;

        #endregion

        #region Build In States

        public override IEnumerator StartManager()
        {
            this.currentSettings = CameraSettings.Default();

            yield return new WaitWhile(() => this.defaultCameraRig is null);

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

        public UnityEngine.Camera GetCurrentCamera()
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