#region Packages

using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Runtime.AI.Senses.Sight;
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

        [SerializeField, FoldoutGroup("Base"), Required]
        private UnitManager unitManager;
        
        [SerializeField, FoldoutGroup("Base/Visual")]
        protected GameObject visualsObject;

        [SerializeField, FoldoutGroup("Base/Navmesh"), Required]
        protected NavMeshAgent agent;

        [SerializeField, FoldoutGroup("Base/Senses")]
        private UnitSight sightCone;

        [SerializeField, FoldoutGroup("Base"), Required] private BehaviourTreeOwner behaviourTreeOwner;

        [SerializeField, FoldoutGroup("Base")] private UnitState unitState;

        private readonly Dictionary<string, object> memoryBank = new();

        private UnityEvent disableEvent;

        private bool previousStoppedState;

        #endregion

        #region Build In States

        private void OnEnable() =>
            this.unitManager.Register(this);

        private void OnDisable() =>
            this.unitManager.Unregister(this);

        #endregion

        #region Getters

        public NavMeshAgent GetAgent() =>
            this.agent;

        public TObject GetFromMemory<TObject>(string key) where TObject : Object
        {
            if (!this.memoryBank.ContainsKey(key)) return null;

            return this.memoryBank[key] as TObject;
        }

        public UnitState GetUnitState() =>
            this.unitState;

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

        public void SetUnitState(UnitState set) =>
            this.unitState = set;

        #endregion

        #region In

        // ReSharper disable Unity.PerformanceAnalysis
        public void UpdateUnit()
        {
            if (this.sightCone != null)
                this.sightCone.UpdateSight();

            if (this.behaviourTreeOwner != null)
                this.behaviourTreeOwner.UpdateBehaviour();
        }

        public abstract void InteractTrigger();

        public void AddDisableEventListener(UnityAction action)
        {
            this.disableEvent ??= new UnityEvent();

            this.disableEvent.AddListener(action);
        }

        public void RemoveDisableEventListener(UnityAction action)
        {
            this.disableEvent?.RemoveListener(action);
        }

        public void PauseUnit()
        {
            this.previousStoppedState = this.agent.isStopped;

            this.agent.isStopped = true;
        }

        public void ResumeUnit()
        {
            this.agent.isStopped = this.previousStoppedState;
        }

        #endregion

        #region Out

        public object GetStateByKey(string s)
        {
            return null;
        }

        #endregion
    }
}