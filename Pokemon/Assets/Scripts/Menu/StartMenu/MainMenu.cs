#region SDK

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
            WorldManager.instance.UnloadSceneAsync("StartMenu");
            
            UIManager.instance.SwitchUI(UISelection.Overworld);
        }

        public void LoadBattleScene(string sceneName)
        {
            WorldManager.instance.LoadBattleScene(sceneName);
            WorldManager.instance.UnloadSceneAsync("StartMenu");
            
            UIManager.instance.SwitchUI(UISelection.Battle);
        }

        public void LoadTestBattle()
        {
           PlayerManager.instance.closestBattleStarter.StartBattleNow();
        }
        
        #endregion
    }
}