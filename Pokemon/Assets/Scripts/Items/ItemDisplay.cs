#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#endregion

public class ItemDisplay : MonoBehaviour
{
    #region Values
    [SerializeField] private Item item;
    [SerializeField] private int count;
    [SerializeField] TextMeshProUGUI nameText = null, countText = null;
    #endregion

    #region Getters
    public string GetItemName()
    {
        return item.GetItemName();
    }
    #endregion

    #region Set
    public void SetItem(Item i)
    {
        item = i;

        nameText.text = item.GetItemName();

        AddCount(1);
    }
    #endregion

    #region In
    public void AddCount(int i)
    {
        count += i;

        countText.text = "" + count;
    }

    public void RemoveCount(int i)
    {
        count -= i;

        countText.text = "" + count;
    }
    #endregion
}