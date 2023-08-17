#region Libraries

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Runtime.Algorithms.PathFinding;
using Runtime.Core;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    [DisallowMultipleComponent]
    public sealed class UnitAgent : MonoBehaviour
    {
        #region Values

        [SerializeField, InlineEditor] private UnitAgentSettings settings;
        
        [SerializeField] private Transform target;

        private Vector3 pre;

        private UnitPath currentPath;

        private int currentTriangleIndex = -1;

        [SerializeField, HideInInspector] private Rigidbody rb;

        private bool pathPending, isOnNavMesh;

        public List<Portal> portals;

        public CalculatedNavMesh navMesh;

        public Dictionary<int, RemappedVert> remappedVerts;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            this.rb = this.rb != null ? this.rb : this.GetComponent<Rigidbody>();
            this.rb.useGravity = false;

            yield return new WaitWhile(() => !UnitNavigation.Ready);

            this.currentTriangleIndex = UnitNavigation.PlaceAgentOnNavMesh(this);

            this.isOnNavMesh = this.currentTriangleIndex != -1;

            if (this.isOnNavMesh)
                this.rb.useGravity = true;
        }

        private void Update()
        {
            if (!this.currentPath.Empty && !this.currentPath.Complete)
                this.currentPath.Tick(this);

            if (this.currentTriangleIndex == -1)
                return;

            if (this.target.position == this.pre || this.currentTriangleIndex == -1)
                return;

            this.pre = this.target.position;

            this.MoveTo(this.pre);
        }

        private void OnDrawGizmos()
        {
            if (this.navMesh != null)
            {
                NavTriangle t = this.navMesh.Triangles[this.navMesh.ClosestTriangleIndex(this.target.position)];
                Debug.DrawRay(t.Center(this.navMesh.Vertices()), Vector3.up);
            }

            this.currentPath.DebugPath(this);
        }

        #endregion

        #region Getters

        public int CurrentTriangleIndex() => this.currentTriangleIndex;

        public UnitAgentSettings Settings => this.settings;

        public bool AgentIsOnNavMesh() => this.isOnNavMesh;

        #endregion

        #region In

        public void MoveTo(Vector3 position)
        {
            if (InTriangle2D(UnitNavigation.GetTriangleByID(this.currentTriangleIndex).Vertices, position))
            {
                return;
            }

            UnitNavigation.QueueForPath(this, position);
        }

        public void MoveToAndFace(Vector3 position, Quaternion direction)
        {
            if (InTriangle2D(UnitNavigation.GetTriangleByID(this.currentTriangleIndex).Vertices, position))
            {
                if (Vector3.Angle(this.transform.forward, direction.ForwardFromRotation()) >
                    this.settings.WalkTurnAngle)
                {
                    //
                }

                return;
            }

            UnitNavigation.QueueForPath(this, position);
        }

        public void SetPath(UnitPath path) =>
            this.currentPath = path;

        internal void MoveAgentBody(Vector3 towards)
        {
            Vector3 position = this.transform.position;
            this.rb.MovePosition(position + this.settings.MoveSpeed * Time.deltaTime *
                (towards.XZ() - position.XZ()).ToV3(0).normalized);
        }

        #endregion

        #region Internal

        private static bool InTriangle2D(int[] corners, Vector3 point)
        {
            if (corners.Length < 3)
                return false;

            Vector2[] positions = UnitNavigation.Get2DVertByIndex(corners);

            return MathC.PointWithinTriangle2D(point.XZ(), positions[0], positions[1], positions[2]);
        }

        #endregion
    }
}