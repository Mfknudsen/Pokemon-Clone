using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { }

[CreateAssetMenu(fileName = "Item", menuName = "Item/Create new standard Item")]
public class Item : ScriptableObject
{
    #region Values
    [Header("Object Reference:")]
    [SerializeField] protected bool isInstantiated = false;

    [Header("Item Information:")]
    [SerializeField] protected ItemType type = 0;
    [SerializeField] protected string itemName = "";
    [SerializeField, TextArea] protected string description = "";
    #endregion

    #region Getters
    public Item GetItem()
    {
        Item result = this;

        if (!result.GetIsInstantiated())
        {
            result = Instantiate(result);
            result.SetIsInstantiated(true);
        }

        return result;
    }

    public ItemType GetItemType()
    {
        return type;
    }

    public bool GetIsInstantiated()
    {
        return isInstantiated;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public string GetDescription()
    {
        return description;
    }
    #endregion

    #region Setters
    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;
    }
    #endregion
}
