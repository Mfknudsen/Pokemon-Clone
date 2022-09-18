#region Packages

using System;
using System.Collections;
using Runtime.Systems.Operation;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle.AfterEffects
{
    [Serializable]
    public abstract class NpcBattleAfterEffect : ScriptableObject, IOperation
    {
        private bool instantiated;
        protected bool done = false;

        #region Getters

        public bool IsOperationDone()
        {
            return done;
        }

        public bool GetIsInstantiated()
        {
            return instantiated;
        }

        #endregion

        #region Setters

        public abstract void SetInput(object input);

        #endregion

        #region In

        public void IsInstantiated()
        {
            instantiated = true;
        }
        
        public abstract IEnumerator Operation();

        public virtual void OperationEnd()
        {
        }

        #endregion

        #region Out

        public abstract NpcBattleAfterEffect GetInstantiatedEffect();

        #endregion
    }
}