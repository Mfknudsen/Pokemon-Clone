#region Libraries

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    public sealed class UnitNavigationAgent : MonoBehaviour
    {
        #region Values

        [SerializeField, InlineEditor]
        private UnitAgentSettings settings;

        [SerializeField]
        private Transform target;

        private Vector3 pre;

        private UnitPath currentPath;

        private int currentTriangleIndex;

        #endregion

        #region Build In States

        private void OnDrawGizmos()
        {
            if (this.currentPath == null)
                return;

        }

        private void OnEnable()
        {

        }

        private void Update()
        {
            if (this.target.position == this.pre)
                return;

            this.pre = this.target.position;

            this.MoveTo(this.pre);
        }

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