#region SDK

using System.Collections.Generic;
using Mfknudsen.Battle.Actions.Item;
using Mfknudsen.Items;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
{
    public class ItemSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private GameObject itemUIPrefab;

        [SerializeField] private List<ItemDisplay> displays = new List<ItemDisplay>();
        [SerializeField] private Item[] list = new Item[0];

        private ItemAction itemAction;

        #endregion

        #region In

        public void Setup()
        {
        }

        public void DisplaySelection(ItemAction itemAction)
        {
            this.itemAction = itemAction;
        }

        public void DisableDisplaySelection()
        {
        }

        #endregion
    }
}