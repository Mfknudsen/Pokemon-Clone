#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Items;
using Mfknudsen.Player;
using Mfknudsen.Player.UI_Book;
using Mfknudsen.Settings.Manager;
using Mfknudsen.UI;
using Mfknudsen.UI.Cursor;
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
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 150;

            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            yield return new WaitWhile(() => !SetupManager.instance);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

            yield return new WaitWhile(() => !asyncOperation.isDone || !Logger.instance || !CustomCursor.instance);

            SetupManager.instance.Trigger();
            CustomCursor.HideCursor();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            Inventory inventory = null;
            while (inventory == null)
            {
                inventory = PlayerManager.instance.GetBattleMember().GetInventory();
                yield return null;
            }

            foreach (Item item in items)
                inventory.AddItem(item);
            
            SetupManager.instance.Trigger();

            yield return new WaitWhile(() => !UIManager.instance ||!UIBook.instance);

            UIManager.instance.SwitchUI(UISelection.Start);
            UIBook.instance.Effect(BookTurn.Open);
        }
    }
}