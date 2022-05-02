#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Items;
using Mfknudsen.Player;
using Mfknudsen.Player.UI_Book;
using Mfknudsen.Settings.Manager;
using Mfknudsen.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Mfknudsen._Debug.Logger;

#endregion

namespace Mfknudsen
{
    public class StartGame : MonoBehaviour
    {
        public List<Item> items;

        private void Start()
        {
            InputManager.Instance = new InputManager();
            
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

            yield return new WaitWhile(() => PlayerManager.instance.GetBattleMember().GetInventory() == null);
            
            Inventory inventory = PlayerManager.instance.GetBattleMember().GetInventory();

            foreach (Item item in items)
                inventory.AddItem(item);
            
            SetupManager.instance.Trigger();

            yield return new WaitWhile(() => !UIManager.instance ||!UIBook.instance);

            UIManager.instance.SwitchUI(UISelection.Start);
        }
    }
}