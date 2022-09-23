#region Packages

using System.Collections;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public abstract class Manager : ScriptableObject
    {
        #region Values

        protected bool ready;
        private bool isStarted;

        #endregion

        #region Getters
        
        public bool GetReady() => this.ready;

        public bool GetIsStarted() => this.isStarted;

        #endregion

        #region Setters

        public void SetIsStarted(bool set) => this.isStarted = set;

        #endregion

        #region In

        public virtual IEnumerator StartManager()
        {
            yield break;
        }

        public virtual void UpdateManager()
        {
        }

        public virtual void FixedUpdateManager()
        {
        }

        #endregion
    }
}