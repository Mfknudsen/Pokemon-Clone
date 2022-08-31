#region Packages

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Runtime.Items
{
    public class Inventory : MonoBehaviour
    {
        #region Values

        [Header("Object Reference:")] [SerializeField]
        private readonly List<ItemContainer> items = new();

        [Header("Bag Categories:")] [SerializeField]
        private List<ItemType> medicine;

        [SerializeField] private List<ItemType> balls, battle, berries, other, tm, treasure, key;

        #endregion

        #region Getters

        public ItemContainer[] GetAllItems()
        {
            return items.ToArray();
        }

        public Item GetItemByIndex(int i)
        {
            return items[i].item;
        }

        public bool IsItemInBag(Item i)
        {
            return items.Any(container => container.item.GetItemName() == i.GetItemName());
        }

        #region - Bag Categories

        public ItemContainer[] GetMedicine()
        {
            return items.Where(i => medicine.Contains(i.item.GetItemType())).ToArray();
        }

        public ItemContainer[] GetPokeballs()
        {
            return items.Where(i => balls.Contains(i.item.GetItemType())).ToArray();
        }

        public ItemContainer[] GetBattleItems()
        {
            return items.Where(i => battle.Contains(i.item.GetItemType())).ToArray();
        }

        public ItemContainer[] GetBerries()
        {
            return items.Where(i => berries.Contains(i.item.GetItemType())).ToArray();
        }

        public ItemContainer[] GetOther()
        {
            return items.Where(i => other.Contains(i.item.GetItemType())).ToArray();
        }

        public ItemContainer[] GetTM()
        {
            return items.Where(i => tm.Contains(i.item.GetItemType())).ToArray();
        }

        public ItemContainer[] GetTreasure()
        {
            return items.Where(i => treasure.Contains(i.item.GetItemType())).ToArray();
        }

        public ItemContainer[] GetKeyItems()
        {
            return items.Where(i => key.Contains(i.item.GetItemType())).ToArray();
        }

        #endregion

        #endregion

        #region Setters

        #endregion

        #region In

        public void AddItem(Item i)
        {
            bool foundExisting = false;
            foreach (ItemContainer container in items.Where(
                container => container.item.GetItemName() == i.GetItemName()))
            {
                foundExisting = true;
                container.count++;
                break;
            }

            if (foundExisting) return;

            ItemContainer itemContainer = new(i);
            items.Add(itemContainer);
        }

        public void RemoveItem(Item i)
        {
            foreach (ItemContainer container in items.Where(
                container => container.item.GetItemName() == i.GetItemName()))
            {
                container.count--;
                break;
            }
        }

        #endregion
    }

    public class ItemContainer
    {
        public ItemContainer(Item item)
        {
            this.item = item;
            count = 1;
        }
        
        public Item item;
        public int count;
    }
}