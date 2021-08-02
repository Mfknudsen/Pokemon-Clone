#region SDK

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

        [SerializeField] private Transform moveOrigin;
        [SerializeField] private float moveSpeed, rotateSpeed;

        [SerializeField] private Transform turnPoint;

        [SerializeField] private Vector3 oldRot;

        #endregion

        #region Build In States

        private void Start()
        {
            agent ??= moveOrigin.GetComponent<NavMeshAgent>();
            agent.enabled = false;

            rb ??= moveOrigin.GetComponent<Rigidbody>();
            rb.useGravity = false;

            playerInput = new PlayerInput();
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

        public void OnMoveAxisChange(InputAction.CallbackContext value)
        {
            playerInput.SetMoveDirection(value.ReadValue<Vector2>());
        }

        public void OnInteractionChange(InputAction.CallbackContext value)
        {
            if (!value.performed) return;

            Debug.Log("Interaction Trigger");
        }

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
            Vector3 forwardMove = transform.forward * playerInput.GetMoveDirection().z;
            Vector3 sideMove = transform.right * playerInput.GetMoveDirection().x;
            
            transform.position += (forwardMove + sideMove).normalized * (moveSpeed * Time.deltaTime);
        }

        private void Turn()
        {
            float angel = playerInput.GetTargetRotation().x - transform.rotation.eulerAngles.x;

            transform.Rotate(Vector3.up, angel * rotateSpeed * Time.deltaTime);
        }

        #endregion
    }
}