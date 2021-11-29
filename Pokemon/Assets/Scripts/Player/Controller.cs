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
        private bool ready = false;

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
            rotateSpeed;

        private Vector3 toLookRotation = Vector3.forward;

        #region AnimatorHashs

        private static readonly int HashWalking = Animator.StringToHash("Walking"),
            HashXMove = Animator.StringToHash("X Move"),
            HashYMove = Animator.StringToHash("Y Move");

        #endregion

        #endregion

        #region Build In States

        private void Update()
        {
            if (!ready) return;

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

            playerInputContainer = PlayerManager.instance.GetPlayerInput();

            ready = true;
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

            animController.SetBool(HashWalking, playerInputDirection.x != 0 || playerInputDirection.y != 0);
            animController.SetFloat(HashXMove, playerInputDirection.x, 0.1f, Time.deltaTime);
            animController.SetFloat(HashYMove, playerInputDirection.z, 0.1f, Time.deltaTime);
        }

        private void Move()
        {
            if (!agent.isOnNavMesh) return;

            Vector2 playerInputDirection = playerInputContainer.GetMoveDirection();
            Vector3 forwardMove = moveTransform.forward * playerInputDirection.y;
            Vector3 sideMove = moveTransform.right * playerInputDirection.x;
            Vector3 moveVector = (forwardMove + sideMove).normalized;

            agent.Move(moveVector * (moveSpeed * Time.deltaTime));
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