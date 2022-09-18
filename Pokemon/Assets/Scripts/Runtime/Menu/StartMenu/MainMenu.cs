#region Packages

using System.Collections;
using Runtime.Player;
using Runtime.Systems.UI;
using Runtime.World;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Menu.StartMenu
{
    public class MainMenu : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private WorldManager worldManager;
        [SerializeField, Required] private UIManager uiManager;
        [SerializeField, Required] private PlayerManager playerManager;
        private bool ready = true;

        #endregion

        #region In

        public void LoadScene(string sceneName)
        {
            if (!ready) return;

            ready = true;

            worldManager.LoadSceneAsync(sceneName);
            uiManager.SwitchUI(UISelection.Overworld);
            playerManager.EnableOverworld();
            worldManager.UnloadSceneAsync("StartMenu");
        }

        public void StartNewGame()
        {
            if (!ready) return;

            ready = true;

            StartCoroutine(StartGame());
        }

        #endregion

        #region Internal

        private IEnumerator StartGame()
        {
            const string sceneName = "Shayklind";

            if (!worldManager.GetCurrentLoadedWorldScene().Equals(sceneName))
                worldManager.LoadSceneAsync(sceneName);

            yield return null;

            yield return new WaitWhile(() => worldManager.GetIsLoading());

            worldManager.UnloadSceneAsync("StartMenu");

            yield return null;

            yield return new WaitWhile(() => worldManager.GetActiveUnloading());
        }

        #endregion
    }
}