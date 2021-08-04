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
            WorldMaster.instance.LoadSceneAsync(sceneName);
            WorldMaster.instance.UnloadSceneAsync("StartMenu");
            
            UIManager.instance.SwitchUI(UISelection.Overworld);
        }

        public void LoadBattleScene(string sceneName)
        {
            WorldMaster.instance.LoadBattleScene(sceneName);
            WorldMaster.instance.UnloadSceneAsync("StartMenu");
            
            UIManager.instance.SwitchUI(UISelection.Battle);
        }

        public void LoadTestBattle()
        {
           PlayerManager.instance.closestBattleStarter.StartBattleNow();
        }
        
        #endregion
    }
}