#region Packages

using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public abstract class Manager : ScriptableObject
    {
        #region Values

        protected ManagerUpdater holder;

        [SerializeField] private bool includeAsInactive;
        private bool ready, isStarted;

        #endregion

        #region Getters

        public bool GetInclude() => includeAsInactive;

        public bool GetReady() => ready;

        public bool GetIsStarted() => isStarted;

        public GameObject GetHolderObject() => this.holder.gameObject;
        
        #endregion

        #region Setters

        public void SetIsStarted(bool set) => isStarted = set;

        public void SetHolder(ManagerUpdater holder) => this.holder = holder;

        #endregion

        #region In

        public virtual void UpdateManager()
        {
        }

        public virtual void FixedUpdateManager()
        {
        }
        
        #endregion
    }
}