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

        public static SetupManager Instance;

        #endregion

        #region Build In States

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        #endregion

        #region In

        public void Trigger()
        {
            List<Manager> managers = FindObjectsOfType<Manager>()
                .Where(m => !m.GetReady())
                .OrderBy(i => i.Priority())
                .ToList();

            foreach (Manager manager in managers)
            {
                manager.Setup();
                manager.Ready();
            }
        }

        #endregion
    }
}