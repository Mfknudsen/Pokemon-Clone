#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Items;
using Runtime.Player;
using Runtime.Systems.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Runtime._Debug.Logger;

#endregion

namespace Runtime
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField, Required] private PlayerManager playerManager;
        [SerializeField, Required] private UIManager uiManager;

        public List<Item> items;

        private IEnumerator Start()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);

            yield return new WaitWhile(() => !asyncOperation.isDone || !Logger.instance);

            while (!this.playerManager.GetBattleMember()?.GetInventory())
            {
                Debug.Log(this.playerManager.GetBattleMember());
                yield return null;
            }

            yield return new WaitWhile(() => !this.playerManager.GetBattleMember()?.GetInventory());

            Inventory inventory = this.playerManager.GetBattleMember().GetInventory();

            foreach (Item item in this.items)
                inventory.AddItem(item);

            this.uiManager.SwitchUI(UISelection.Start);
        }
    }
}