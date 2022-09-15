﻿#region Packages

using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

#endregion

namespace Runtime.AI
{
    public class NpcController : MonoBehaviour
    {
        #region Values

        [SerializeField] private UnitManager unitManager;
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
            unitManager.AddController(this);
        }

        private void OnDisable()
        {
            unitManager.RemoveController(this);
        }

        #endregion

        #region Getters

        public object GetStateByKey(string key)
        {
            return stateList.ContainsKey(key) ? stateList[key] : null;
        }

        #endregion

        #region Setters

        public void SetState(string key, object value)
        {
            if (stateList.ContainsKey(key))
                stateList[key] = value;
            else
                stateList.Add(key, value);
        }

        #endregion

        #region In

        public void TriggerBehaviourUpdate()
        {
            bto.UpdateBehaviour();
        }

        #endregion
    }
}