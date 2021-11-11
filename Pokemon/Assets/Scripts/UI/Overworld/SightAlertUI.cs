#region Packages

using Mfknudsen.Battle.Systems;
using Mfknudsen.UI.Overworld.Sight_Alerts;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Overworld
{
    public class SightAlertUI : MonoBehaviour
    {
        #region In

        public void EnableAlert(AlertType alertType)
        {
            alertType.Trigger(transform, true);
            OperationsContainer container = new OperationsContainer();
            container.Add(alertType);
            OperationManager.instance.AddAsyncOperationsContainer(container);
        }

        public void DisableAlert(AlertType alertType)
        {
            alertType.Trigger(transform, false);
            OperationsContainer container = new OperationsContainer();
            container.Add(alertType);
            OperationManager.instance.AddAsyncOperationsContainer(container);
        }

        #endregion
    }
}