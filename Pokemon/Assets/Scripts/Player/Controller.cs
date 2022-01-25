#region Packages

using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.Player
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

        [FoldoutGroup("Animation")] [SerializeField]
        private float animatorDamp = 0.1f;

        private PlayerInputContainer playerInputContainer;
        private bool ready, allowed;

        private Vector3 toLookRotation = Vector3.forward;

        private float yCamSpeed, xCamSpeed;

        #region Hashs

        private static readonly int HashWalking = Animator.StringToHash("WalkSpeed");

        #endregion

        #endregion

        #region Build In States

        private void Update()
        {
            if (!ready || !allowed) return;

            UpdateMoveTransform();
            Move();
            Turn();
            UpdateAnimController();
        }

        #endregion

        #region Setters

        #endregion

        #region In

        public void Setup()
        {
            playerTransform = transform;

            agent ??= playerTransform.GetComponent<NavMeshAgent>();

            rb ??= playerTransform.GetComponent<Rigidbody>();
            rb.useGravity = false;

            playerInputContainer = PlayerManager.instance.GetPlayerInput();

            yCamSpeed = cameraRig.m_YAxis.m_MaxSpeed;
            xCamSpeed = cameraRig.m_XAxis.m_MaxSpeed;

            ready = true;
        }

        public void Enable()
        {
            allowed = true;

            cameraRig.m_YAxis.m_MaxSpeed = yCamSpeed;
            cameraRig.m_XAxis.m_MaxSpeed = xCamSpeed;
        }

        public void Disable()
        {
            allowed = false;

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
            moveTransform.rotation = Quaternion.Euler(0, camTransform.localRotation.eulerAngles.y, 0);
        }

        private void UpdateAnimController()
        {
            if (animController == null) return;

            Vector3 playerInputDirection = playerInputContainer.GetMoveDirection();

            animController.SetFloat(
                HashWalking,
                playerInputDirection.magnitude * 2 * (playerInputContainer.GetRun() ? 3 : 1),
                animatorDamp,
                Time.deltaTime);
        }

        private void Move()
        {
            if (agent == null || !agent.isOnNavMesh) return;

            Vector2 playerInputDirection = playerInputContainer.GetMoveDirection();
            Vector3 forwardMove = moveTransform.forward * playerInputDirection.y;
            Vector3 sideMove = moveTransform.right * playerInputDirection.x;
            Vector3 moveVector = (forwardMove + sideMove).normalized;

            agent.Move(moveVector * ((playerInputContainer.GetRun() ? runSpeed : moveSpeed) * Time.deltaTime));
        }

        private void Turn()
        {
            if (playerInputContainer.GetMoveDirection() != Vector2.zero)
            {
                Vector2 playerInputDirection = playerInputContainer.GetMoveDirection();
                Vector3 forwardMove = moveTransform.forward * playerInputDirection.y;
                Vector3 sideMove = moveTransform.right * playerInputDirection.x;
                toLookRotation = (forwardMove + sideMove).normalized;
            }

            visualTransform.rotation = Quaternion.Lerp(visualTransform.rotation,
                Quaternion.LookRotation(toLookRotation), rotateSpeed * Time.deltaTime);
        }

        #endregion
    }
}