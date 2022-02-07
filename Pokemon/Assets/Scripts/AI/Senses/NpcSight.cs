#region Packages

using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Player;
using Mfknudsen.UI.Overworld;
using Mfknudsen.UI.Overworld.Sight_Alerts;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Senses
{
    public class NpcSight : MonoBehaviour
    {
        #region Values

        public readonly List<OperationsContainer> onSeenOperations = new List<OperationsContainer>();

        [SerializeField] private AlertType alertType;
        [SerializeField] private float povAngel = 60;
        [SerializeField] private Transform origin;
        [SerializeField] private float distance = 5;

        #endregion

        #region Build In States

        private void Update()
        {
            Vector3 dir = (PlayerManager.instance.transform.position - origin.transform.position).normalized;

            float actualAngel = Vector3.Angle(origin.forward, dir);
            if (actualAngel > povAngel / 2) return;

            if (!Physics.Raycast(origin.transform.position, dir, out RaycastHit hit, distance)) return;

            if (hit.collider.CompareTag("Player")) return;

            if (onSeenOperations == null) return;

            OperationManager operationManager = OperationManager.instance;
            foreach (OperationsContainer onSeenOperation in onSeenOperations)
                operationManager.AddOperationsContainer(onSeenOperation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            SightAlertUI.instance.EnableAlert(alertType);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            SightAlertUI.instance.DisableAlert(alertType);
        }

        #endregion
    }
}