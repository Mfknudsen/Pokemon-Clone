#region Packages

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.Settings.Manager
{
    public abstract class Manager : MonoBehaviour, ISetup
    {
        #region Values

        [FoldoutGroup("ISetup")] [SerializeField]
        protected int priority = 1;

        private bool ready;

        #endregion

        #region Getters

        public int Priority()
        {
            return priority;
        }

        public bool GetReady()
        {
            return ready;
        }

        #endregion

        #region In

        public abstract void Setup();

        public void Ready()
        {
            ready = true;
        }

        #endregion
    }
}