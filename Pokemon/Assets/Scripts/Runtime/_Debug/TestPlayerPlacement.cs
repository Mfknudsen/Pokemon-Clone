#region Packages

using Runtime.Systems;
using Runtime.Systems.PersistantRunner;
using UnityEngine;

#endregion

namespace Runtime._Debug
{
    public class TestPlayerPlacement : MonoBehaviour
    {
#if UNITY_EDITOR

        #region Values

        [SerializeField] private Manager[] addToPersistant;

        #endregion

        private void Start()
        {
            this.SetupPersistantRunner();
        }

        #region Internal

        private void SetupPersistantRunner()
        {
            PersistantRunner persistantRunner = new GameObject().AddComponent<PersistantRunner>();

            foreach (Manager manager in this.addToPersistant)
                persistantRunner.AddManager(manager);

            persistantRunner.StartManagers();
        }

        #endregion

#endif
    }
}