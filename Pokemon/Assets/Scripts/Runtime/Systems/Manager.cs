#region Packages

using System.Collections;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public abstract class Manager : ScriptableObject
    {
        #region Values

        [SerializeField] private bool includeAsInactive;
        protected bool ready;
        private bool isStarted;

        #endregion

        #region Getters

        public bool GetInclude() => includeAsInactive;

        public bool GetReady() => ready;

        public bool GetIsStarted() => isStarted;

        #endregion

        #region Setters

        public void SetIsStarted(bool set) => isStarted = set;

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