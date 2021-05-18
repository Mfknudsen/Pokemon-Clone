#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Monster;
#endregion

#region Enums
public enum ItemType { Berry, MegaStone, Revive, Potion, }
#endregion

public class Item : ScriptableObject
{
    #region Values
    [Header("Object Reference:")]
    [SerializeField] protected bool isInstantiated = false;

    [Header("Item Information:")]
    [SerializeField] protected ItemType type = 0;
    [SerializeField] protected string itemName = "";
    [SerializeField, TextArea] protected string description = "";

    [Header("Operation:")]
    [SerializeField] protected Pokemon target = null;
    [SerializeField] protected bool inUse = false;
    [SerializeField] protected bool active = false, done = false;
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

    public bool GetInUse()
    {
        return inUse;
    }

    public bool GetDone()
    {
        return done;
    }

    public virtual bool IsUsableTarget(Pokemon p)
    {
        Debug.Log("Is Usable Target Need Override");
        return false;
    }
    #endregion

    #region Setters
    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;
    }

    public void SetInUse(bool set)
    {
        inUse = set;
    }

    public virtual void SetTarget(Pokemon set)
    {
        target = set;
    }
    #endregion

    #region Out
    public virtual IEnumerator Activate()
    {
        active = true;

        Debug.Log("Activate Need Override");
        yield return null;

        done = true;
    }
    #endregion
}