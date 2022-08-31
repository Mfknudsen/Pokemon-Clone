#region Packages

using Runtime.Systems;
using Runtime.UI.Overworld.Sight_Alerts;
using UnityEngine;

#endregion

namespace Runtime.UI.Overworld
{
    public class SightAlertUI : MonoBehaviour
    {
        public static SightAlertUI instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else 
                Destroy(gameObject);
        }

        #region In

        public void EnableAlert(AlertType alertType)
        {
            alertType.Trigger(transform, true);
            OperationsContainer container = new();
            container.Add(alertType);
            OperationManager.instance.AddAsyncOperationsContainer(container);
        }

        public void DisableAlert(AlertType alertType)
        {
            alertType.Trigger(transform, false);
            OperationsContainer container = new();
            container.Add(alertType);
            OperationManager.instance.AddAsyncOperationsContainer(container);
        }

        #endregion
    }
}