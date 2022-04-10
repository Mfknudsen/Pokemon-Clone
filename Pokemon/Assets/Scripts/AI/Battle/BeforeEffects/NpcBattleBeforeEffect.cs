#region Packages

using System;
using System.Collections;
using Mfknudsen.Battle.Systems;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Battle.AfterEffects
{
    [Serializable]
    public abstract class NpcBattleBeforeEffect : ScriptableObject, IOperation
    {
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