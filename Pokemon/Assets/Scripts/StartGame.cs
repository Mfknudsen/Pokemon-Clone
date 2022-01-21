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

#endregion

namespace Mfknudsen
{
    public class StartGame : MonoBehaviour
    {
        public List<Item> items;

        private void Start()
        {
            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            while (SetupManager.Instance == null)
                yield return null;
            
            SetupManager.Instance.Trigger();

            Inventory inventory = null;
            while (inventory == null)
            {
                inventory = PlayerManager.Instance.GetBattleMember().GetInventory();
                yield return null;
            }

            foreach (Item item in items)
            {
                inventory.AddItem(item);
            }

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

            while (!asyncOperation.isDone)
                yield return null;
            
            SetupManager.Instance.Trigger();

            while (UIManager.Instance == null)
                yield return null;

            UIManager.Instance.SwitchUI(UISelection.Start);
            UIBook.Instance.Effect(BookTurn.Open);
        }
    }
}