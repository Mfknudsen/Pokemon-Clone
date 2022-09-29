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
    [RequireComponent(typeof(NpcController))]
    public abstract class UnitBase : MonoBehaviour, IInteractable
    {
        #region Values

        [SerializeField, FoldoutGroup("Base"), Required]
        private UnitManager unitManager;

        [SerializeField, FoldoutGroup("Base")]
        protected Chat idleChat;

        [FoldoutGroup("Base/Visual")] protected GameObject visualsObject;

        [FoldoutGroup("Base/Navmesh")] protected NavMeshAgent agent;

        private Dictionary<string, object> memoryBank = new();

        private UnityEvent disableEvent;

        private NpcController controller;

        #endregion

        #region Build In States

        private void OnEnable()
        {
            this.unitManager.AddController(this.controller);
        }

        private void OnDisable()
        {
            this.unitManager.RemoveController(this.controller);
        }

        #endregion

        #region Getters

        public NavMeshAgent GetAgent()
        {
            return this.agent;
        }

        public TObject GetFromMemory<TObject>(string key) where TObject : Object
        {
            if (!this.memoryBank.ContainsKey(key)) return null;

            return this.memoryBank[key] as TObject;
        }

        #endregion

        #region Setters

        public void SetMemory(string key, object value)
        {
            if (this.memoryBank.ContainsKey(key))
            {
                this.memoryBank[key] = value;
                return;
            }

            this.memoryBank.Add(key, value);
        }

        #endregion

        #region In

        public abstract void Trigger();

        public void HideVisual()
        {
            this.visualsObject.SetActive(false);
        }

        public void ShowVisual()
        {
            this.visualsObject.SetActive(true);
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