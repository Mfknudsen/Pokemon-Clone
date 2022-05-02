#region Packages

using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.Settings.Manager
{
    public abstract class Manager : MonoBehaviour, ISetup
    {
        #region Values

        [SerializeField] private bool includeAsInactive;
        private bool ready, isStarted;

        #endregion

        #region Getters

        public bool GetInclude()
        {
            return includeAsInactive;
        }

        public bool GetReady()
        {
            return ready;
        }

        public bool GetIsStarted()
        {
            return isStarted;
        }

        #endregion

        #region Setters

        public void SetIsStarted(bool set)
        {
            isStarted = set;
        }

        #endregion

        #region In

        public abstract IEnumerator Setup();

        #endregion
    }
}