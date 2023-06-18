#region Libraries

using UnityEngine;

#endregion

namespace Runtime.AI.World.Navigation
{
    public sealed class UnitNavigationAgent : MonoBehaviour
    {
        #region Values

        [SerializeField]
        private UnitAgentSettings settings;

        private UnitPath currentPath;

        private int currentTriangleIndex;

        #endregion

        #region Getters

        public int CurrentTriangleIndex => this.currentTriangleIndex;

        public UnitAgentSettings Settings => this.settings;

        #endregion

        #region In

        public void MoveTo(Vector3 position)
        {
            UnitNavigation.QueueForPath(this, position);
        }

        public void MoveToAndFace(Vector3 position, Quaternion direction)
        {
            UnitNavigation.QueueForPath(this, position);
        }

        #endregion
    }
}