#region Packages

using System.Collections;
using Cinemachine;
using Runtime.Battle.Systems;
using Runtime.Settings;
using Runtime.Systems.Operation;
using Sirenix.OdinInspector;
using UnityEngine;
// ReSharper disable ParameterHidesMember

#endregion

namespace Runtime.Player.Camera
{
    [CreateAssetMenu]
    public class CameraEvent : ScriptableObject, IOperation
    {
        #region Values

        [SerializeField, Required] private CameraManager cameraManager;

        private bool done;
        private CinemachineVirtualCameraBase cinemachineRig;
        private CameraSettings cameraSettings;
        private float percentToEnable, timeInSeconds;

        public CameraEvent Setup(
            CinemachineVirtualCameraBase cinemachineRig,
            CameraSettings cameraSettings,
            float timeInSeconds,
            float percentToEnable = 0)
        {
            //percentToEnable must be between 0 and 1
            this.cinemachineRig = cinemachineRig;
            this.percentToEnable = percentToEnable;
            this.timeInSeconds = timeInSeconds;
            this.cameraSettings = cameraSettings ?? CameraSettings.Default();

            return this;
        }

        #endregion

        #region Build In States

        #endregion

        #region In

        public bool IsOperationDone()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            yield return new WaitForSeconds(timeInSeconds * percentToEnable);

            cameraManager.SetCurrentRig(cinemachineRig, true);
            if (cameraSettings != null)
                cameraManager.SetCameraSettings(cameraSettings);

            yield return new WaitForSeconds(timeInSeconds * (1 - percentToEnable));

            done = true;
        }

        public void OperationEnd()
        {
        }

        #endregion

        #region Out

        public static CameraEvent ReturnToDefaultOverworld(PlayerManager playerManager)
        {
            return CreateInstance<CameraEvent>().Setup(
                playerManager.GetOverworldCameraRig(),
                Setting.OverworldCameraSettings,
                2,
                0.75f
            );
        }

        public static CameraEvent ReturnToDefaultBattle()
        {
            return CreateInstance<CameraEvent>().Setup(
                BattleManager.instance.GetBattleCamera(),
                Setting.BattleCameraSettings,
                0,
                1
            );
        }

        #endregion
    }
}