#region SDK

using System.Collections.Generic;
using Mfknudsen.Items;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.UI.Selection
{
    public class ItemSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private GameObject itemUIPrefab;

        [SerializeField] private List<ItemDisplay> displays = new List<ItemDisplay>();
        [SerializeField] private Item[] list = new Item[0];

        #endregion

        #region In

        public void Setup()
        {
        }

        public void DisplaySelection()
        {
        }

        public void DisableDisplaySelection()
        {
        }

        #endregion
    }
}