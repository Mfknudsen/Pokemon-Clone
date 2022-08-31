#region Packages

using System.Collections;
using Cinemachine;
using Runtime.Battle.Systems;
using Runtime.Settings;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Player.Camera
{
    public struct CameraEvent : IOperation
    {
        #region Values

        private bool done;
        private readonly CinemachineVirtualCameraBase cinemachineRig;
        private readonly CameraSettings cameraSettings;
        private readonly float percentToEnable, timeInSeconds;

        public CameraEvent(
            CinemachineVirtualCameraBase cinemachineRig,
            CameraSettings cameraSettings,
            float timeInSeconds,
            float percentToEnable) : this()
        {
            //percentToEnable must be between 0 and 1
            this.cinemachineRig = cinemachineRig;
            this.percentToEnable = percentToEnable;
            this.timeInSeconds = timeInSeconds;
            this.cameraSettings = cameraSettings ?? CameraSettings.Default();
        }

        #endregion

        #region In

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            CameraManager cameraManager = CameraManager.instance;

            yield return new WaitForSeconds(timeInSeconds * percentToEnable);

            cameraManager.SetCurrentRig(cinemachineRig, true);
            if (cameraSettings != null)
                cameraManager.SetCameraSettings(cameraSettings);

            yield return new WaitForSeconds(timeInSeconds * (1 - percentToEnable));

            done = true;
        }

        public void End()
        {
        }

        #endregion

        #region Out

        public static CameraEvent ReturnToDefaultOverworld()
        {
            return new CameraEvent(
                PlayerManager.instance.GetOverworldCameraRig(),
                Setting.OverworldCameraSettings,
                2,
                0.75f
            );
        }

        public static CameraEvent ReturnToDefaultBattle()
        {
            return new CameraEvent(
                BattleManager.instance.GetBattleCamera(),
                Setting.BattleCameraSettings,
                0,
                1
            );
        }

        #endregion
    }
}