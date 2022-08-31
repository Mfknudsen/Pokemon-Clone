#region Packages

using Runtime.Battle.Actions;
using Runtime.Battle.UI.Selection;
using TMPro;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Items
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