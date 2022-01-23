#region Packages

using System.Collections;
using Cinemachine;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Settings;
using UnityEngine;

#endregion

namespace Mfknudsen.Player.Camera
{
    public struct CameraEvent : IOperation
    {
        #region Values

        private bool done;
        private readonly bool resetOnEnd;
        private readonly CinemachineVirtualCameraBase cinemachineRig;
        private readonly CameraSettings cameraSettings;
        private readonly float percentToEnable, timeInSeconds;

        public CameraEvent(
            CinemachineVirtualCameraBase cinemachineRig,
            CameraSettings cameraSettings,
            float timeInSeconds,
            float percentToEnable,
            bool? resetOnEnd = null) : this()
        {
            //percentToEnable must be between 0 and 1
            this.cinemachineRig = cinemachineRig;
            this.percentToEnable = percentToEnable;
            this.timeInSeconds = timeInSeconds;
            this.resetOnEnd = resetOnEnd ?? false;
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
            if (resetOnEnd)
                CameraManager.instance.Reset();
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