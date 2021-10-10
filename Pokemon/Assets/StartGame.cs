#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Items;
using Mfknudsen.Player;
using Mfknudsen.Trainer;
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
            Inventory inventory = null;
            while (inventory == null)
            {
                inventory = PlayerManager.instance.GetBattleMember().GetInventory();
                yield return null;
            }

            foreach (Item item in items)
            {
                inventory.AddItem(item);
            }

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

            while (!asyncOperation.isDone)
                yield return null;

            while (UIManager.instance == null)
                yield return null;

            UIManager.instance.SwitchUI(UISelection.Start);
        }
    }
}