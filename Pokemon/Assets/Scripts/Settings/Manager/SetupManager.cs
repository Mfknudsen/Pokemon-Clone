#region Packages

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mfknudsen.Settings.Manager
{
    public class SetupManager : MonoBehaviour
    {
        #region Values

        public static SetupManager instance;

        #endregion

        #region Build In States

        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region In

        public void Trigger()
        {
            Manager[] managers = FindObjectsOfType<Manager>(true)
                .Where(m =>
                    !(!m.gameObject.activeInHierarchy && !m.GetInclude()) &&
                    !m.GetReady() &&
                    !m.GetIsStarted())
                .ToArray();

            foreach (Manager manager in managers)
            {
                manager.SetIsStarted(true);
                StartCoroutine(manager.Setup());
            }
        }

        #endregion
    }
}