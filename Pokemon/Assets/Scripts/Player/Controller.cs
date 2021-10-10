#region SDK

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Player
{
    public class Controller : MonoBehaviour
    {
        #region Values

        private PlayerInput playerInput;

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Animator animController;

        [SerializeField] private Transform playerTransform, moveTransform;
        [SerializeField] private float moveSpeed, rotateSpeed;

        #region AnimatorHashs

        private static readonly int Walking = Animator.StringToHash("Walking");
        private static readonly int XMove = Animator.StringToHash("X Move");
        private static readonly int YMove = Animator.StringToHash("Y Move");

        #endregion

        #endregion

        #region Build In States

        private void Start()
        {
            playerTransform = transform;

            agent ??= playerTransform.GetComponent<NavMeshAgent>();
            agent.enabled = false;

            rb ??= playerTransform.GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        private void Update()
        {
            Move();
            Turn();
        }

        #endregion

        #region Getters

        public PlayerInput GetPlayerInput()
        {
            return playerInput;
        }

        #endregion

        #region Setters

        #endregion

        #region In

        #region Input

        [UsedImplicitly]
        public void OnMoveAxisChange(InputAction.CallbackContext value)
        {
            playerInput.SetMoveDirection(value.ReadValue<Vector2>());
        }

        [UsedImplicitly]
        public void OnMouseAxisChange(InputAction.CallbackContext value)
        {
            Vector3 vec = transform.rotation.eulerAngles;

            vec.x += value.ReadValue<Vector2>().x;
            vec.y += value.ReadValue<Vector2>().y;

            playerInput.SetTargetRotation(vec);
        }

        #endregion

        #endregion

        #region Out

        #endregion

        #region Internal

        private void Move()
        {
            Camera camObject = Camera.main;
            
            if (camObject == null)
                return;

            Transform camTransform = camObject.transform;

            moveTransform.rotation = Quaternion.Euler(0, camTransform.localRotation.eulerAngles.y, 0);

            Vector3 playerInputDirection = playerInput.GetMoveDirection();

            Vector3 forwardMove = moveTransform.forward * playerInputDirection.z;
            Vector3 sideMove = moveTransform.right * playerInputDirection.x;

            playerTransform.position += (forwardMove + sideMove).normalized * (moveSpeed * Time.deltaTime);

            animController.SetBool(Walking, playerInputDirection.x != 0 || playerInputDirection.y != 0);
            animController.SetFloat(XMove, playerInputDirection.x, 0.1f, Time.deltaTime);
            animController.SetFloat(YMove, playerInputDirection.z, 0.1f, Time.deltaTime);
        }

        private void Turn()
        {
            if(playerInput.GetMoveDirection().normalized == Vector3.zero) return;
            Vector3 targetVector = Quaternion.LookRotation(playerInput.GetMoveDirection().normalized).eulerAngles;

            float angel = playerInput.GetTargetRotation().x - targetVector.x;

            playerTransform.Rotate(Vector3.up, angel * rotateSpeed * Time.deltaTime);
        }

        #endregion
    }
}