#region Packages

using System;
using System.Collections;
using Runtime.Communication;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle.BeforeEffects
{
    [Serializable]
    public abstract class NpcBattleBeforeEffect : ScriptableObject, IOperation
    {
        [SerializeField, Required] protected ChatManager chatManager;
        
        private bool instantiated;
        protected bool done = false;

        #region Getters

        public bool Done()
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

        public virtual void End()
        {
        }

        #endregion

        #region Out

        public abstract NpcBattleBeforeEffect GetInstantiatedEffect();

        #endregion
    }
}