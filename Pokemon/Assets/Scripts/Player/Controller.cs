#region Packages

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.Player
{
    public class Controller : MonoBehaviour
    {
        #region Values

        private PlayerInputContainer playerInputContainer;
        private bool ready, allowed;

        [SerializeField] [FoldoutGroup("Components")]
        private NavMeshAgent agent;

        [SerializeField] [FoldoutGroup("Components")]
        private Rigidbody rb;

        [SerializeField] [FoldoutGroup("Components")]
        private Animator animController;

        [SerializeField] [FoldoutGroup("Transforms")]
        private Transform playerTransform,
            moveTransform,
            camTransform,
            visualTransform;

        [SerializeField] [FoldoutGroup("Speeds")]
        private float moveSpeed,
            rotateSpeed,
            runSpeed;

        [SerializeField] [FoldoutGroup("Animation")]
        private float animatorDamp = 0.1f;

        private Vector3 toLookRotation = Vector3.forward;

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

        #region Getters

        public PlayerInputContainer GetPlayerInput()
        {
            return playerInputContainer;
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

            playerInputContainer = PlayerManager.Instance.GetPlayerInput();

            ready = true;
        }

        public void Enable()
        {
            allowed = true;
        }

        public void Disable()
        {
            allowed = false;
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