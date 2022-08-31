using UnityEngine;

namespace Runtime.Weathers
{
    [System.Serializable]
    public class Weather : ScriptableObject
    {
        #region Values

        [SerializeField] private string effectName;
        [SerializeField] private int maxEffectTurns;
        private int currentEffectTurn;
        protected bool amplified;

        #region Visuals

        #endregion

        #endregion

        #region Getters

        public string GetEffectName()
        {
            return effectName;
        }

        #endregion

        #region In

        public virtual void Setup()
        {
        }

        public void TickTurn()
        {
            currentEffectTurn++;
        }

        #endregion

        #region Out

        public bool EffectDone()
        {
            return currentEffectTurn == maxEffectTurns;
        }

        #endregion
    }
}