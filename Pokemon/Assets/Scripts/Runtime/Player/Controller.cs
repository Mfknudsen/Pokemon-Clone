#region Packages

using Cinemachine;
using Runtime.ScriptableVariables.Structs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Runtime.Player
{
    public class Controller : MonoBehaviour
    {
        #region Values

        [FoldoutGroup("Components")] [SerializeField]
        private NavMeshAgent agent;

        [FoldoutGroup("Components")] [SerializeField]
        private Rigidbody rb;

        [FoldoutGroup("Components")] [SerializeField]
        private Animator animController;

        [FoldoutGroup("Components")] [SerializeField]
        private CinemachineFreeLook cameraRig;

        [FoldoutGroup("Transforms")] [SerializeField]
        private Transform playerTransform,
            moveTransform,
            camTransform,
            visualTransform;

        [FoldoutGroup("Speeds")] [SerializeField]
        private float moveSpeed,
            rotateSpeed,
            runSpeed;

        [HorizontalGroup("Speeds/Camera")] [SerializeField]
        private float yCamSpeed, xCamSpeed;

        [FoldoutGroup("Animation")] [SerializeField]
        private float animatorDamp = 0.1f;

        [FoldoutGroup("Variables")] [SerializeField]
        private BoolVariable aiming, allowed, running;

        [FoldoutGroup("Variables")] [SerializeField]
        private Vec2Variable moveDirection, rotationDirection;

        private bool ready;

        private Vector3 toLookRotation = Vector3.forward;

        #region Hashs

        private static readonly int HashWalking = Animator.StringToHash("WalkSpeed");

        #endregion

        #endregion

        #region Build In States

        private void Update()
        {
            if (!ready || allowed.Equals(false)) return;

            UpdateMoveTransform();
            Move();
            Turn();
            UpdateAnimController();
        }

        #endregion

        #region Getters

        public GameObject GetVisual() => visualTransform.gameObject;

        #endregion

        #region In

        public void Setup()
        {
            playerTransform = transform;

            agent ??= playerTransform.GetComponent<NavMeshAgent>();

            rb ??= playerTransform.GetComponent<Rigidbody>();
            rb.useGravity = false;

            ready = true;
        }

        public void Enable()
        {
            allowed.value = true;

            cameraRig.m_YAxis.m_MaxSpeed = yCamSpeed;
            cameraRig.m_XAxis.m_MaxSpeed = xCamSpeed;
        }

        public void Disable()
        {
            allowed.value = false;

            cameraRig.m_YAxis.m_MaxSpeed = 0;
            cameraRig.m_XAxis.m_MaxSpeed = 0;
        }

        public void TriggerAnimator(int triggerID)
        {
            animController.SetTrigger(triggerID);
        }

        #endregion

        #region Internal

        private void UpdateMoveTransform()
        {
            moveTransform.LookAt(moveTransform.position + camTransform.forward, Vector3.up);
        }

        private void UpdateAnimController()
        {
            if (animController == null) return;

            Vector3 playerInputDirection = moveDirection.value;

            animController.SetFloat(
                HashWalking,
                playerInputDirection.magnitude * 2 * (running.value ? 3 : 1),
                animatorDamp,
                Time.deltaTime);
        }

        private void Move()
        {
            if (agent == null || !agent.isOnNavMesh) return;

            Vector2 playerInputDirection = moveDirection.value;
            Vector3 forwardMove = moveTransform.forward * playerInputDirection.y;
            Vector3 sideMove = moveTransform.right * playerInputDirection.x;
            Vector3 moveVector = (forwardMove + sideMove).normalized;

            agent.Move(moveVector * ((running.value ? runSpeed : moveSpeed) * Time.deltaTime));
        }

        private void Turn()
        {
            if (this.aiming.Equals(false))
            {
                if (moveDirection.value != Vector2.zero)
                {
                    Vector2 playerInputDirection = moveDirection.value;
                    Vector3 forwardMove = moveTransform.forward * playerInputDirection.y;
                    Vector3 sideMove = moveTransform.right * playerInputDirection.x;
                    this.toLookRotation = (forwardMove + sideMove).normalized;
                    this.toLookRotation -= new Vector3(0, this.toLookRotation.y, 0);
                }

                this.visualTransform.rotation = Quaternion.Lerp(this.visualTransform.rotation,
                    Quaternion.LookRotation(this.toLookRotation), this.rotateSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.up, rotationDirection.x * 5 * Time.deltaTime);
            }
        }

        #endregion
    }
}