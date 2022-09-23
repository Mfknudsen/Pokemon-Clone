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
            if (!this.ready) return;

            this.ready = true;

            this.worldManager.LoadSceneAsync(sceneName);
            this.uiManager.SwitchUI(UISelection.Overworld);
            this.playerManager.EnableOverworld();
            this.worldManager.UnloadSceneAsync("StartMenu");
        }

        public void StartNewGame()
        {
            if (!this.ready) return;

            this.ready = true;

            StartCoroutine(StartGame());
        }

        #endregion

        #region Internal

        private IEnumerator StartGame()
        {
            const string sceneName = "Shayklind";

            if (!this.worldManager.GetCurrentLoadedWorldScene().Equals(sceneName)) this.worldManager.LoadSceneAsync(sceneName);

            yield return null;

            yield return new WaitWhile(() => this.worldManager.GetIsLoading());

            this.worldManager.UnloadSceneAsync("StartMenu");

            yield return null;

            yield return new WaitWhile(() => this.worldManager.GetActiveUnloading());
        }

        #endregion
    }
}