#region Packages

using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI
{
    public class NpcController : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private UnitManager unitManager;
        [SerializeField] private BehaviourTreeOwner bto;

        private Dictionary<string, object> stateList = new()
        {
            { "spottedDanger", false },
            { "spottedGift", false },
            { "warnTime", 0.0f },
            { "stunned", false }
        };

        #endregion

        #region Build In States

        private void OnEnable()
        {
            this.unitManager.AddController(this);
        }

        private void OnDisable()
        {
            this.unitManager.RemoveController(this);
        }

        #endregion

        #region Getters

        public object GetStateByKey(string key)
        {
            return this.stateList.ContainsKey(key) ? this.stateList[key] : null;
        }

        #endregion

        #region Setters

        public void SetState(string key, object value)
        {
            if (this.stateList.ContainsKey(key))
                this.stateList[key] = value;
            else
                this.stateList.Add(key, value);
        }

        #endregion

        #region In

        public void TriggerBehaviourUpdate()
        {
            this.bto.UpdateBehaviour();
        }

        #endregion
    }
}