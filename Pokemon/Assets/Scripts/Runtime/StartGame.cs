 #region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Items;
using Runtime.Player;
using Runtime.Systems;
using Runtime.UI;
using Runtime.UI_Book;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Runtime._Debug.Logger;

#endregion

namespace Runtime
{
    public class StartGame : MonoBehaviour
    {
        public List<Item> items;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 150;
            
            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            yield return new WaitWhile(() => !SetupManager.instance);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

            yield return new WaitWhile(() => !asyncOperation.isDone || !Logger.instance);

            SetupManager.instance.Trigger();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            yield return new WaitWhile(() => !PlayerManager.instance.GetBattleMember().GetInventory());
            
            Inventory inventory = PlayerManager.instance.GetBattleMember().GetInventory();

            foreach (Item item in this.items)
                inventory.AddItem(item);
            
            SetupManager.instance.Trigger();

            yield return new WaitWhile(() => !UIManager.instance ||!UIBook.instance);

            UIManager.instance.SwitchUI(UISelection.Start);
        }
    }
}