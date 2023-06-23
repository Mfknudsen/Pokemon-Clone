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

        private int currentTriangleIndex;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            this.GetComponent<Rigidbody>().useGravity = false;

            yield return new WaitWhile(() => !UnitNavigation.ready);

            Vector2 p = this.target.position.XZ();
            this.currentTriangleIndex = UnitNavigation.ClosestTriangleIndex(p);
            NavTriangle t = UnitNavigation.GetTriangleByID(this.currentTriangleIndex);
            Vector2[] corners = UnitNavigation.Get2DVertByIndex(t.Vertices);

            if (!ExtMathf.PointWithinTriangle2D(p, corners[0], corners[1], corners[2]))
            {
                this.transform.position =
                    Vector3.Lerp(
                        UnitNavigation.Get3DVertByIndex(t.Vertices[0]),
                    Vector3.Lerp(
                        UnitNavigation.Get3DVertByIndex(t.Vertices[2]),
                        UnitNavigation.Get3DVertByIndex(t.Vertices[2]), .5f), .5f);
            }
            else
                this.transform.position =
                    new Vector3(this.transform.position.x, t.MaxY, this.transform.position.z);

            this.GetComponent<Rigidbody>().useGravity = true;
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