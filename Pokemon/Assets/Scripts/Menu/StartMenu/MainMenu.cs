#region Packages

using System.Collections;
using Mfknudsen.Player;
using Mfknudsen.UI;
using Mfknudsen.World;
using UnityEngine;

#endregion

namespace Mfknudsen.Menu.StartMenu
{
    public class MainMenu : MonoBehaviour
    {
        #region Values

        private bool ready = true;

        #endregion
        
        #region In

        public void LoadScene(string sceneName)
        {
            if(!ready) return;

            ready = true;
            
            WorldManager.instance.LoadSceneAsync(sceneName);

            UIManager.instance.SwitchUI(UISelection.Overworld);

            PlayerManager.instance.EnableOverworld();

            WorldManager.instance.UnloadSceneAsync("StartMenu");
        }

        public void StartNewGame()
        {
            if(!ready) return;

            ready = true;

            StartCoroutine(StartGame());
        }

        #endregion

        #region Internal

        private static IEnumerator StartGame()
        {
            const string sceneName = "Shayklind";
            
            if (!WorldManager.instance.GetCurrentLoadedWorldScene().Equals(sceneName))
                WorldManager.instance.LoadSceneAsync(sceneName);

            yield return null;
            
            yield return new WaitWhile(() => WorldManager.instance.GetIsLoading());
            
            WorldManager.instance.UnloadSceneAsync("StartMenu");

            yield return null;

            yield return new WaitWhile(() => WorldManager.instance.GetActiveUnloading());
            
            
        }

        #endregion
    }
}