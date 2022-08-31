#region Packages

using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Spots;
using Runtime.Items;
using Runtime.Player;
using UnityEngine;

#endregion

// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable ConvertIfStatementToSwitchExpression
// ReSharper disable ParameterHidesMember
namespace Runtime.Battle.UI.Selection
{
    public class ItemSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private SelectionMenu selectionMenu;
        [SerializeField] private GameObject background;

        [SerializeField] private GameObject itemDisplay, bagSlotSelection;

        [SerializeField] private GameObject itemUIPrefab;
        [SerializeField] private Transform parentTransform;
        [SerializeField] private List<ItemDisplay> displays = new();

        private readonly List<ItemContainer> potionList = new(),
            pokéballList = new(),
            otherList = new(),
            medicineList = new();

        private SpotOversight spotOversight;
        private ItemAction itemAction;
        private Inventory playerInventory;

        #endregion

        #region In

        public void Setup()
        {
            playerInventory = PlayerManager.instance.GetBattleMember().GetInventory();
            spotOversight = BattleManager.instance.GetSpotOversight();
        }

        public void DisplaySelection(ItemAction itemAction)
        {
            background.SetActive(true);

            this.itemAction = itemAction;

            foreach (ItemContainer itemContainer in playerInventory.GetAllItems()
                .Where(container => container.item is BattleItem))
            {
                BattleBagSlot slot = ((BattleItem)itemContainer.item).GetBattleBagSlot();

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (slot == BattleBagSlot.Battle && !potionList.Contains(itemContainer))
                    potionList.Add(itemContainer);
                else if (slot == BattleBagSlot.Pokéball && !pokéballList.Contains(itemContainer))
                    pokéballList.Add(itemContainer);
                else if (slot == BattleBagSlot.Berries && !otherList.Contains(itemContainer))
                    otherList.Add(itemContainer);
                else if (slot == BattleBagSlot.Medicine && !medicineList.Contains(itemContainer))
                    medicineList.Add(itemContainer);
            }

            itemDisplay.SetActive(false);
            bagSlotSelection.SetActive(true);
        }

        public void DisplayByBagSlot(int i)
        {
            BattleBagSlot battleBagSlot = (BattleBagSlot)i;

            bagSlotSelection.SetActive(false);
            itemDisplay.SetActive(true);

            List<ItemContainer> toDisplay;

            if (battleBagSlot == BattleBagSlot.Battle)
                toDisplay = potionList;
            else if (battleBagSlot == BattleBagSlot.Pokéball)
                toDisplay = pokéballList;
            else if (battleBagSlot == BattleBagSlot.Berries)
                toDisplay = otherList;
            else
                toDisplay = medicineList;

            foreach (ItemContainer itemContainer in toDisplay)
            {
                ItemDisplay display = Instantiate(itemUIPrefab, parentTransform).GetComponent<ItemDisplay>();

                display.Setup(this, itemContainer, itemAction);

                displays.Add(display);
            }
        }

        public void DisableDisplaySelection()
        {
            foreach (ItemDisplay display in displays)
                Destroy(display.gameObject);

            displays.Clear();

            background.SetActive(false);
        }

        public void ReceiveAction(BattleAction battleAction)
        {
            if (spotOversight.GetToDefaultTargeting())
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Spot spot in spotOversight.GetSpots())
                {
                    bool enemy = spot.GetActivePokemon() != battleAction.GetCurrentPokemon();

                    if (!battleAction.GetDefaultTargetEnemy() && enemy ||
                        battleAction.GetDefaultTargetEnemy() && !enemy) continue;

                    battleAction.SetTargets(spot.GetActivePokemon());

                    battleAction.GetCurrentPokemon().SetBattleAction(battleAction);

                    break;
                }
            }
            else
                selectionMenu.DisplaySelection(SelectorGoal.Target, battleAction);
        }

        #endregion
    }
}