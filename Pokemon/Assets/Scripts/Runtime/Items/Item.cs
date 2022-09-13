﻿#region Packages

using System.Collections;
using Runtime.Communication;
using Runtime.Pokémon;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Items
{
    #region Enums

    public enum ItemType
    {
        Berry,
        MegaStone,
        Revive,
        Potion,
    }

    public enum BattleBagSlot
    {
        Battle,
        Pokéball,
        Berries,
        Medicine
    }

    #endregion

    public abstract class Item : ScriptableObject, IOperation
    {
        #region Values

        [Header("Object Reference:")] [SerializeField]
        protected bool isInstantiated;

        [Header("Item Information:")] [SerializeField]
        protected ItemType type = 0;

        [SerializeField] protected string itemName = "";
        [SerializeField, TextArea] protected string description = "";

        [Header("Operation:")] [SerializeField]
        protected Pokemon target;

        [SerializeField] protected bool inUse;
        [SerializeField] protected bool done;

        #region Visual

        [Header("Visual")] [SerializeField] private GameObject visualPrefab;
        private GameObject instantiateObject;

        #endregion

        #endregion

        #region Getters

        public Item GetItem()
        {
            Item result = this;

            if (result.GetIsInstantiated()) return result;

            result = Instantiate(result);
            result.SetIsInstantiated(true);

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

        public abstract bool IsUsableTarget(Pokemon pokemon);

        public GameObject GetInstantiatedVisualObject()
        {
            return instantiateObject;
        }

        public GameObject GetVisualPrefab() => this.visualPrefab;

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

        #region In

        public void SpawnVisualObject()
        {
            instantiateObject = Instantiate(visualPrefab);
        }

        #endregion

        #region IOperation

        public bool Done()
        {
            return done;
        }

        public abstract IEnumerator Operation();

        public virtual void End()
        {
        }

        #endregion
    }
}