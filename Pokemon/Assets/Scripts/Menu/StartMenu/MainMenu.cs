#region Packages

using Mfknudsen.Player;
using Mfknudsen.UI;
using Mfknudsen.World;
using UnityEngine;

#endregion

namespace Mfknudsen.Menu.StartMenu
{
    public class MainMenu : MonoBehaviour
    {
        #region In

        public void LoadScene(string sceneName)
        {
            WorldManager.instance.LoadSceneAsync(sceneName);

            UIManager.Instance.SwitchUI(UISelection.Overworld);

            PlayerManager.Instance.EnableOverworld();

            WorldManager.instance.UnloadSceneAsync("StartMenu");
        }

        public void LoadBattleScene(string sceneName)
        {
            WorldManager.instance.LoadBattleScene(sceneName);

            UIManager.Instance.SwitchUI(UISelection.Battle);
            
            WorldManager.instance.UnloadSceneAsync("StartMenu");
        }

        #endregion
    }
}