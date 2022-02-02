#region Packages

using System;
using System.Collections;
using Mfknudsen.Battle.Systems;

#endregion

namespace Mfknudsen.AI.Battle.AfterEffects
{
    [Serializable]
    public abstract class NpcBattleBeforeEffect : IOperation
    {
        protected bool done = false;

        #region Getters

        public bool Done()
        {
            return done;
        }

        #endregion

        #region Setters

        public abstract void SetInput(object input);

        #endregion

        #region In

        public abstract IEnumerator Operation();

        public virtual void End()
        {
        }

        #endregion
    }
}