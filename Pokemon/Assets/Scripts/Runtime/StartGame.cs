#region Packages

using System.Collections;
using Runtime.Player;
using Runtime.Systems.UI;
using Runtime.UI_Book;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Runtime
{
    public class StartGame : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private PlayerManager playerManager;
        [SerializeField, Required] private UIManager uiManager;

        #endregion

        private IEnumerator Start()
        {
            Application.targetFrameRate = 60;
            Debug.Log("Starting Game");
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

            yield return new WaitWhile(() => !asyncOperation.isDone);

            yield return new WaitUntil(() => this.playerManager.Ready && this.uiManager.Ready);

            this.playerManager.DisablePlayerControl();

            this.uiManager.SwitchUI(UISelection.Start);
            UIBook.Instance.ConstructUI();

            Debug.Log("Game Started");
        }
    }
}