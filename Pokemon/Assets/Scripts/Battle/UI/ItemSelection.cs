#region SDK

using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Monster;
using UnityEngine;
using UnityEngine.UI; //Custom

#endregion

namespace Mfknudsen.Battle.UI
{
    public class ItemSelection : MonoBehaviour
    {
        #region Values
        [Header("Object Reference:")]
        [SerializeField] private GameObject itemUIPrefab = null;
        [SerializeField] private Transform origin = null;
        [SerializeField] private ItemType[] useableInBattle = new ItemType[0];
        [SerializeField] private List<ItemDisplay> displays = new List<ItemDisplay>();
        [SerializeField] private Item[] list = new Item[0];

        [SerializeField] private Item toSend = null;
        #endregion

        #region In
        public void Setup(Item[] l)
        {
            foreach (ItemDisplay d in displays)
                Destroy(d.gameObject);
            displays.Clear();

            list = l;

            foreach (Item i in list)
            {
                if (i == null)
                    continue;
                bool check = false, exist = false;

                foreach (ItemType type in useableInBattle)
                {
                    if (i.GetItemType() == type)
                    {
                        check = true;
                        break;
                    }
                }

                if (!check)
                    continue;

                foreach (ItemDisplay d in displays)
                {
                    if (d.GetItemName() == i.GetItemName())
                    {
                        d.AddCount(1);
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    GameObject obj = Instantiate(itemUIPrefab);
                    obj.transform.parent = origin;
                    obj.transform.position = origin.position;


                    ItemDisplay display = obj.GetComponent<ItemDisplay>();
                    display.SetItem(i);
                    displays.Add(display);

                    obj.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        ItemClicked(display.GetItemName());
                    });
                }
            }
        }

        public void ItemClicked(string itemName)
        {
            foreach (Item i in list)
            {
                if (i.GetItemName() == itemName)
                {
                    toSend = i;
                    BattleMaster.instance.ShowPokemonSelector(SelectorGoal.Item, toSend);
                    break;
                }
            }
        }

        public void ReceiveTarget(Pokemon target)
        {
            toSend.SetInUse(true);
            BattleMaster.instance.SelectItem(toSend, target);
        }
        #endregion
    }
}