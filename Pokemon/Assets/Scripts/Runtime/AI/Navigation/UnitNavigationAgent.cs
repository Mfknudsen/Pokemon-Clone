#region Libraries

using Runtime.Common;
using Sirenix.OdinInspector;
using System.Collections;
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

        private int currentTriangleIndex = -1;

        [SerializeField] private CalculatedNavMesh calculatedNavMesh;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            this.GetComponent<Rigidbody>().useGravity = false;

            yield return new WaitWhile(() => !UnitNavigation.Ready);

            this.currentTriangleIndex = UnitNavigation.PlaceAgentOnNavMesh(this);

            this.GetComponent<Rigidbody>().useGravity = true;
        }

        private void Update()
        {
            if (this.currentTriangleIndex == -1)
                return;

            if (this.target.position == this.pre || this.currentTriangleIndex == -1)
                return;

            this.pre = this.target.position;

            this.MoveTo(this.pre);
        }

        private void OnDrawGizmos()
        {
            if (this.target != null)
            {
                int targetID = this.calculatedNavMesh.ClosestTriangleIndex(this.target.position);
                int[] targetIDs = this.calculatedNavMesh.Triangles[targetID].Vertices;
                if (ExtMathf.PointWithinTriangle2D(this.target.position.XZ(),
                    this.calculatedNavMesh.SimpleVertices[targetIDs[0]],
                    this.calculatedNavMesh.SimpleVertices[targetIDs[1]],
                    this.calculatedNavMesh.SimpleVertices[targetIDs[2]]))
                    Debug.DrawRay(new(this.target.position.x, this.calculatedNavMesh.Triangles[targetID].MaxY, this.target.position.z), Vector3.up, Color.red);
                else
                    Debug.DrawRay(this.calculatedNavMesh.Triangles[targetID].Center(this.calculatedNavMesh.Vertices()), Vector3.up, Color.red);
            }

            int id = this.calculatedNavMesh.ClosestTriangleIndex(this.transform.position);
            int[] ids = this.calculatedNavMesh.Triangles[id].Vertices;
            if (ExtMathf.PointWithinTriangle2D(this.transform.position.XZ(),
                this.calculatedNavMesh.SimpleVertices[ids[0]],
                this.calculatedNavMesh.SimpleVertices[ids[1]],
                this.calculatedNavMesh.SimpleVertices[ids[2]]))
                Debug.DrawRay(new(this.transform.position.x, this.calculatedNavMesh.Triangles[id].MaxY, this.transform.position.z), Vector3.up, Color.red);
            else
                Debug.DrawRay(this.calculatedNavMesh.Triangles[id].Center(this.calculatedNavMesh.Vertices()), Vector3.up, Color.red);


            if (this.currentPath.Empty)
                return;
        }

        #endregion

        #region Getters

        public int CurrentTriangleIndex => this.currentTriangleIndex;

        public UnitAgentSettings Settings => this.settings;

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
                if (Vector3.Angle(this.transform.forward, direction.ForwardFromRotation()) > this.settings.WalkTurnAngle)
                {
                    //
                }

                return;
            }

            UnitNavigation.QueueForPath(this, position);
        }

        public void SetPath(UnitPath path)
        {
        }

        #endregion

        #region Internal

        private static bool InTriangle2D(int[] corners, Vector3 point)
        {
            if (corners.Length < 3)
                return false;

            Vector2[] positions = UnitNavigation.Get2DVertByIndex(corners);

            return ExtMathf.PointWithinTriangle2D(point.XZ(), positions[0], positions[1], positions[2]);
        }

        #endregion
    }
}