#region Packages

using System;
using System.Collections;
using System.Linq;
using Runtime.Common;
using Runtime.Player;
using Runtime.Systems;
using Runtime.Systems.PersistantRunner;
using Runtime.Systems.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Runtime._Debug
{
#if UNITY_EDITOR
    public sealed class TestPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Manager[] managers;

        private IEnumerator Start()
        {
            PlayerManager playerManager = this.managers.First(m => m is PlayerManager) as PlayerManager;
            UIManager uiManager = this.managers.First(m => m is UIManager) as UIManager;

            if (playerManager == null)
                throw new Exception("Missing Player Manager");
            if (uiManager == null)
                throw new Exception("Missing UI Manager");

            Application.targetFrameRate = 60;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

            Transform t = this.transform;
            Vector3 position = t.position;
            Quaternion rotation = t.rotation;
            Instantiate(this.playerPrefab, position, rotation);
            GameObject persistent = new("Persistent Runner");
            persistent.AddComponent<TimerUpdater>();
            PersistantRunner runner = persistent.AddComponent<PersistantRunner>();
            runner.AddManagers(this.managers);

            yield return new WaitWhile(() =>
                !asyncOperation.isDone);

            yield return new WaitUntil(() =>
                playerManager.Ready && uiManager.Ready);

            uiManager.SwitchUI(UISelection.Overworld);
            uiManager.SetReadyToPause(false);
            playerManager.EnablePlayerControl();
            playerManager.SetState(PlayerState.Default);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
#endif
}