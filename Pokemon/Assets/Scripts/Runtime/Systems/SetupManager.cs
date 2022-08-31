#region Packages

using System.Linq;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public class SetupManager : MonoBehaviour
    {
        #region Values

        public static SetupManager instance;

        [SerializeField] private bool t;

        #endregion

        #region Build In States

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            if (t)
                Trigger();
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