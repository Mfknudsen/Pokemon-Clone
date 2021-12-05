#region Packages

using System.Collections;
using Cinemachine;
using Mfknudsen.Battle.Systems;
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
        private UnityEngine.Camera mainCam;

        [FoldoutGroup("Camera")] [SerializeField]
        private CinemachineFreeLook defaultCameraRig;

        private CinemachineVirtualCameraBase currentRig;

        #endregion

        #region In

        public override void Setup()
        {
            instance = this;

            if (mainCam == null)
                mainCam = UnityEngine.Camera.main;
            currentRig = defaultCameraRig;
            defaultCameraRig.enabled = true;
        }

        public void SetCurrentRig(CinemachineVirtualCameraBase set)
        {
            currentRig.enabled = false;
            set.enabled = true;
            currentRig = set;
        }

        public void SetCameraSettings(CameraSettings cameraSettings)
        {
        }

        #endregion
    }

    public struct CameraEvent : IOperation
    {
        private bool done;
        private readonly CinemachineVirtualCameraBase cinemachineRig;
        private readonly CameraSettings cameraSettings;

        public CameraEvent(CinemachineVirtualCameraBase cinemachineRig, CameraSettings? cameraSettings) : this()
        {
            this.cinemachineRig = cinemachineRig;
            if (cameraSettings != null) this.cameraSettings = cameraSettings;

            done = false;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            CameraManager cameraManager = CameraManager.instance;
            if (cameraSettings != null)
                cameraManager.SetCameraSettings(cameraSettings);

            done = true;

            yield break;
        }

        public void End()
        {
        }
    }

    public class CameraSettings
    {
    }
}