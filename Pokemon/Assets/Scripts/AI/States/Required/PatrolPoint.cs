using Sirenix.OdinInspector;
using UnityEngine;

namespace Mfknudsen.AI.States.Required
{
    public class PatrolPoint : MonoBehaviour
    {
        #region Values

        [BoxGroup("Idle")] public bool idleAtPoint { get; }

        [BoxGroup("Idle")]
        [HideIf("idleAtPoint")]
        public float idleTime { get; }

        private Transform pointTransform;

        #endregion

        public Transform GetPointTransform()
        {
            if (pointTransform == null)
                pointTransform = transform;

            return pointTransform;
        }
    }
}