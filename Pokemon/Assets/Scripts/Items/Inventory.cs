#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace Trainer
{
    public class Inventory : MonoBehaviour
    {
        #region Values
        [Header("Object Reference:")]
        [SerializeField] private List<Item> items = new List<Item>();

        [Header("Bag Categories:")]
        [SerializeField] private List<ItemType> medicine, balls, battle, berries, other, tm, treasure, key;
        #endregion

        #region Getters
        public Item[] GetAllItems()
        {
            return items.ToArray();
        }

        public Item GetItemByIndex(int i)
        {
            return items[i];
        }
        #region - Bag Categories
        public Item[] GetMedicine()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (medicine.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        public Item[] GetPokeballs()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (balls.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        public Item[] GetBattleItems()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (battle.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        public Item[] GetBerries()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (berries.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        public Item[] GetOther()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (other.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        public Item[] GetTM()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (tm.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        public Item[] GetTreasure()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (treasure.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        public Item[] GetKeyItems()
        {
            List<Item> result = new List<Item>();

            foreach (Item i in items)
            {
                if (key.Contains(i.GetItemType()))
                    items.Add(i);
            }

            return result.ToArray();
        }
        #endregion
        #endregion

        #region Setters
        #endregion

        #region In
        public void AddItem(Item i)
        {
            items.Add(i);
        }

        public void RemoveItem(Item i)
        {
            if (items.Contains(i))
                items.Remove(i);
        }
        #endregion
    }
}
