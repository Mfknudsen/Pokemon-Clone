#region Packages

using Mfknudsen.Battle.Systems;
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

            UIManager.instance.SwitchUI(UISelection.Overworld);

            PlayerManager.instance.EnableOverworld();

            WorldManager.instance.UnloadSceneAsync("StartMenu");
        }

        public void LoadBattleScene()
        {
            GameObject.Find("NPC Base").GetComponent<BattleStarter>().StartBattleNow();
        }

        #endregion
    }
}