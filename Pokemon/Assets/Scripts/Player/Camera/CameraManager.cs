#region Packages

using System;
using System.Collections;
using Cinemachine;
using Mfknudsen.Settings.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.Player.Camera
{
    public class CameraManager : Manager
    {
        #region Values

        public static CameraManager instance;

        [FoldoutGroup("Camera")] [SerializeField]
        private UnityEngine.Camera currentCamera;

        [FoldoutGroup("Camera")] [SerializeField]
        private CinemachineFreeLook defaultCameraRig;

        private CinemachineVirtualCameraBase currentRig;
        private CameraSettings currentSettings;

        #endregion

        #region Getters

        public CinemachineFreeLook GetDefaultRig()
        {
            return defaultCameraRig;
        }

        public CinemachineVirtualCameraBase GetCurrentRig()
        {
            return currentRig;
        }

        public UnityEngine.Camera GetCurrentCamera()
        {
            return currentCamera;
        }

        public CameraSettings GetCurrentSettings()
        {
            return currentSettings;
        }

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

            currentSettings = CameraSettings.Default();
            currentRig = defaultCameraRig;
            defaultCameraRig.enabled = true;

            yield break;
        }

        public void SetCurrentRigToDefault()
        {
            currentRig.enabled = false;
            defaultCameraRig.enabled = true;
            currentRig = defaultCameraRig;
        }

        public void SetCurrentRig(CinemachineVirtualCameraBase set, bool disablePrevious = false)
        {
            currentRig.enabled = !disablePrevious;

            if (set != null)
                set.enabled = true;

            currentRig = set;
        }

        public void SetCameraSettings(CameraSettings cameraSettings)
        {
            currentSettings = cameraSettings;
            currentCamera.fieldOfView = cameraSettings.FOV;
        }

        public void Reset()
        {
        }

        #endregion
    }
}