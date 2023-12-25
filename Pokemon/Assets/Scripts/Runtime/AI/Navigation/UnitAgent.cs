#region Libraries

using Sirenix.OdinInspector;
using System.Collections;
using Runtime.AI.Navigation.PathActions;
using Runtime.Core;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.AI.Navigation
{
    [DisallowMultipleComponent]
    public sealed class UnitAgent : MonoBehaviour
    {
        #region Values

        [SerializeField, InlineEditor, Required]
        private UnitAgentSettings settings;

        private UnitPath currentPath;

        private int currentTriangleIndex = -1;

        [SerializeField, HideInInspector] private Rigidbody rb;

        private bool pathPending, isOnNavMesh, isStopped;

        private UnityEvent onPathComplete;

        private const float AVOIDANCE_CHECK_STEP = .2f;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            this.onPathComplete = new UnityEvent();
            this.rb ??= this.GetComponent<Rigidbody>();
            this.rb.useGravity = false;

            yield return new WaitWhile(() => !UnitNavigation.Ready);

            this.currentTriangleIndex = UnitNavigation.PlaceAgentOnNavMesh(this);

            this.isOnNavMesh = this.currentTriangleIndex != -1;

            if (this.isOnNavMesh)
                this.rb.useGravity = true;
        }

        private void OnEnable() =>
            UnitNavigation.AddAgent(this);

        private void OnDisable() =>
            UnitNavigation.RemoveAgent(this);

        #endregion

        #region Getters

        public int CurrentTriangleIndex() => this.currentTriangleIndex;

        public UnitAgentSettings Settings => this.settings;


        public bool IsStopped() => this.isStopped;

        #endregion

        #region Setters

        public void SetStopped(bool set) => this.isStopped = set;

        #endregion

        #region In

        internal void UpdateAgent()
        {
            if (this.currentPath.Empty || this.currentPath.Complete)
                return;

            if (this.currentPath.GetCurrentPathAction() is WalkAction walkAction)
            {
                Vector3 pos = this.transform.position;
                Vector3 currentMoveVector = walkAction.Destination() - pos;
                currentMoveVector = this.AgentAvoidance(currentMoveVector);

                if (Vector3.Angle(this.transform.forward, currentMoveVector) < this.settings.WalkTurnAngle)
                {
                    this.rb.MovePosition(pos + currentMoveVector.normalized * this.settings.MoveSpeed *
                        Time.deltaTime);
                }
            }
            else if (this.currentPath.GetCurrentPathAction() is InteractAction interactAction)
            {
            }

            this.currentPath.CheckIndex(this);

            if (this.currentPath.Complete)
                this.onPathComplete.Invoke();
        }

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

        #endregion

        #region Internal

        /// <summary>
        /// Attempt to avoid walking into other agents
        /// </summary>
        /// <param name="currentMoveDirection">Current move direction from the agent to it's destination</param>
        /// <returns>Offset move vector</returns>
        private Vector3 AgentAvoidance(Vector3 currentMoveDirection)
        {
            Vector3 tempForward = currentMoveDirection.FastNorm(),
                tempRight = new Vector3(-tempForward.y, tempForward.x);
            //Check in front
            if (!Physics.BoxCast(this.transform.position + tempForward * this.settings.Radius * 2,
                    new Vector3(this.settings.Radius, this.settings.Radius, this.settings.Radius),
                    tempForward,
                    out RaycastHit hit,
                    this.transform.rotation,
                    this.settings.Radius * 2,
                    UnitNavigation.GetUnitAgentLayerMask(),
                    QueryTriggerInteraction.Ignore))
                return currentMoveDirection;

            //If agent in front is walking the same way then continue
            if (Vector3.Angle(hit.transform.forward, tempForward) < 25)
                return currentMoveDirection;

            for (float i = 0; i <= 1; i += AVOIDANCE_CHECK_STEP)
            {
                //Check right
                if (Physics.BoxCast(
                        this.transform.position + (tempForward + tempRight * i).normalized *
                        this.settings.Radius * 2,
                        new Vector3(this.settings.Radius, this.settings.Radius, this.settings.Radius),
                        this.transform.forward + tempRight * i,
                        out hit,
                        this.transform.rotation,
                        this.settings.Radius * 2,
                        UnitNavigation.GetUnitAgentLayerMask(),
                        QueryTriggerInteraction.Ignore))
                {
                    //If agent on the right is walking the same way then continue
                    if (Vector3.Angle(hit.transform.forward, tempForward) < 25)
                        return currentMoveDirection + tempRight * -i;
                }
                else
                    //If right is clear then continue that way
                    return currentMoveDirection + tempRight * i;

                //Check left
                if (Physics.BoxCast(
                        this.transform.position + (tempForward + tempRight * -i).normalized *
                        this.settings.Radius * 2,
                        new Vector3(this.settings.Radius, this.settings.Radius, this.settings.Radius),
                        this.transform.forward + tempRight * -i,
                        out hit,
                        this.transform.rotation,
                        this.settings.Radius * 2,
                        UnitNavigation.GetUnitAgentLayerMask(),
                        QueryTriggerInteraction.Ignore))
                {
                    //If agent on the left is walking the same way then continue
                    if (Vector3.Angle(hit.transform.forward, tempForward) < 25)
                        return currentMoveDirection + tempRight * -i;
                }
                else
                    //If left is clear then continue that way
                    return currentMoveDirection + tempRight * -i;
            }

            //If there isn't found a better path then default to going right around
            return currentMoveDirection + tempRight;
        }

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