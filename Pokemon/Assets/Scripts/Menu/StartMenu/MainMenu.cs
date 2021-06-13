using Mfknudsen.World;
using UnityEngine;

namespace Mfknudsen.Menu.StartMenu
{
    public class MainMenu : MonoBehaviour
    {
        #region Values

        #endregion

        #region In
        #endregion

        #region Out
        public void LoadScene(string sceneName)
        {
            WorldMaster.instance.LoadScene(sceneName);
        }
        #endregion
    }
}
