#region Libraries

using UnityEngine;

#endregion

namespace Runtime.AI.World.Navigation
{
    public class UnitNavigationAgent : MonoBehaviour
    {
        #region Values

        [SerializeField]
        private float moveSpeed, turnSpeed, turnAngle;

        [SerializeField]
        private bool canTurnInPlace;

        private UnitPath currentPath;

        #endregion

        #region In

        public void MoveTo(Vector3 position)
        {

        }

        public void MoveToAndFace(Vector3 position, Quaternion direction)
        {

        }

        #endregion
    }
}