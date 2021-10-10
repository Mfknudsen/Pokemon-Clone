#region Packages

using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Pokémon;
using TMPro;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Items
{
    public class ItemDisplay : MonoBehaviour
    {
        #region Values

        [SerializeField] TextMeshProUGUI nameText, countText;
        private ItemSelection itemSelection;
        private ItemContainer itemContainer;
        private ItemAction itemAction;

        #endregion

        #region Set

        public void Setup(ItemSelection itemSelection, ItemContainer itemContainer, ItemAction itemAction)
        {
            this.itemSelection = itemSelection;
            this.itemContainer = itemContainer;
            this.itemAction = itemAction;

            nameText.text = itemContainer.item.GetItemName();
            countText.text = "" + itemContainer.count;
        }

        #endregion

        #region In

        public void Trigger()
        {
            itemAction.SetToUse(itemContainer.item);
            itemSelection.ReceiveAction(itemAction);
        }

        #endregion
    }
}