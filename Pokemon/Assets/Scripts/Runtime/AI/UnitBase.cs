#region Packages

using System.Collections.Generic;
using Runtime.Communication;
using Runtime.World.Overworld.Interactions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

#endregion

namespace Runtime.AI
{
    public abstract class UnitBase : MonoBehaviour, IInteractable
    {
        #region Values

        [FoldoutGroup("Base")] [SerializeField]
        protected Chat idleChat;

        [FoldoutGroup("Base/Visual")] protected GameObject visualsObject;

        [FoldoutGroup("Base/Navmesh")] protected NavMeshAgent agent;

        private Dictionary<string, object> memoryBank = new();

        private UnityEvent disableEvent;

        #endregion

        #region Getters

        public NavMeshAgent GetAgent()
        {
            return agent;
        }

        public TObject GetFromMemory<TObject>(string key) where TObject : Object
        {
            if (!memoryBank.ContainsKey(key)) return null;

            return memoryBank[key] as TObject;
        }

        #endregion

        #region Setters

        public void SetMemory(string key, object value)
        {
            if (memoryBank.ContainsKey(key))
            {
                memoryBank[key] = value;
                return;
            }

            memoryBank.Add(key, value);
        }

        #endregion

        #region In

        public abstract void Trigger();

        public void HideVisual()
        {
            visualsObject.SetActive(false);
        }

        public void ShowVisual()
        {
            visualsObject.SetActive(true);
        }

        public void AddDisableEventListener(UnityAction action)
        {
            this.disableEvent ??= new UnityEvent();

            this.disableEvent.AddListener(action);
        }

        public void RemoveDisableEventListener(UnityAction action)
        {
            this.disableEvent?.RemoveListener(action);
        }

        public void DisableUnit()
        {
            
        }

        #endregion
    }
}