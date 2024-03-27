#region Libraries

using System.Collections;
using Runtime.AI.Navigation.PathActions;
using Runtime.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.AI.Navigation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public sealed class UnitAgent : MonoBehaviour
    {
        #region Values

        [SerializeField] [InlineEditor] [Required]
        private UnitAgentSettings settings;

        private UnitPath currentPath;

        private int currentTriangleIndex = -1;

        [SerializeField] [HideInInspector] private Rigidbody rb;

        private bool pathPending, isOnNavMesh, isStopped;

        private UnityEvent onPathComplete;

        private const float AVOIDANCE_CHECK_STEP = .2f;

        #endregion

        #region Build In States

        private void Reset()
        {
            this.rb = this.gameObject.GetComponent<Rigidbody>();
            this.rb.useGravity = false;
            this.rb.constraints = RigidbodyConstraints.FreezeRotation;

            CapsuleCollider capsuleCollider = this.GetComponent<CapsuleCollider>();
            capsuleCollider.center = Vector3.up;
            capsuleCollider.height = 2;
        }

        private IEnumerator Start()
        {
            this.onPathComplete = new UnityEvent();

            yield return new WaitUntil(() => UnitNavigation.Ready);

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

        public bool IsOnNavMesh() => this.isOnNavMesh;

        #endregion

        #region Setters

        public void SetStopped(bool set) => this.isStopped = set;

        #endregion

        #region In

        internal void UpdateAgent()
        {
            if (this.currentPath.Empty || this.currentPath.Complete)
                return;

            PathAction pathAction = this.currentPath.GetCurrentPathAction();
            if (pathAction.IsWalkAction())
            {
                Vector3 pos = this.transform.position;
                Vector3 currentMoveVector = pathAction.Destination() - pos;
                currentMoveVector = this.AgentAvoidance(currentMoveVector);

                if (Vector3.Angle(this.transform.forward, currentMoveVector) < this.settings.WalkTurnAngle)
                    this.rb.MovePosition(pos + currentMoveVector.normalized * this.settings.MoveSpeed *
                        Time.deltaTime);
            }
            else
            {
            }

            this.currentPath.CheckIndex(this);

            if (this.currentPath.Complete)
                this.onPathComplete.Invoke();
        }

        public void MoveTo(Vector3 position)
        {
            if (this.InCurrentTriangle(position))
            {
                NavTriangle t = UnitNavigation.GetTriangleByID(this.currentTriangleIndex);
                MathC.PointWithinTriangle2D(position.XZ(),
                    UnitNavigation.Get2DVertByIndex(t.GetA),
                    UnitNavigation.Get2DVertByIndex(t.GetB),
                    UnitNavigation.Get2DVertByIndex(t.GetC),
                    out float w1, out float w2);
                Debug.Log($"{w1}  |  {w2}");
            }

            if (this.InCurrentTriangle(position)) return;

            UnitNavigation.QueueForPath(this, position);
        }

        public void MoveToAndFace(Vector3 position, Quaternion direction)
        {
            if (this.InCurrentTriangle(position))
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

        internal void Place(Vector3 position)
        {
            this.transform.position =
                position + Vector3.up * this.GetComponent<CapsuleCollider>().center.y;

            this.rb.velocity = Vector3.zero;
        }

        #endregion

        #region Internal

        /// <summary>
        ///     Attempt to avoid walking into other agents
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
                {
                    return currentMoveDirection + tempRight * i;
                }

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
                {
                    return currentMoveDirection + tempRight * -i;
                }
            }

            //If there isn't found a better path then default to going right around
            return currentMoveDirection + tempRight;
        }

        private bool InCurrentTriangle(Vector3 point)
        {
            NavTriangle t = UnitNavigation.GetTriangleByID(this.currentTriangleIndex);

            return MathC.PointWithinTriangle2D(point.XZ(),
                UnitNavigation.Get2DVertByIndex(t.GetA),
                UnitNavigation.Get2DVertByIndex(t.GetB),
                UnitNavigation.Get2DVertByIndex(t.GetC));
        }

        #endregion
    }
}