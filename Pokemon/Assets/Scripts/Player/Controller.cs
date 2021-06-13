#region SDK

using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.Player
{
    public class Controller : MonoBehaviour
    {
        #region Values
        [Header("Object Reference:")]
        [SerializeField] private NavMeshAgent agent = null;
        [SerializeField] private Rigidbody rb = null;

        [Header("World Condtions:")]
        [SerializeField] private bool onBoat = false;
        [SerializeField] private bool inSandstorm = false, inWater = false, climbStone = false, climbIce = false;

        [Header("Movement:")]
        [SerializeField] private Transform moveOrigin = null;
        [SerializeField] private float speed = 0;
        [SerializeField] private Vector3 moveDir = Vector3.zero;
        [Header(" - Climbing:")]

        [Header("Turn")]
        [SerializeField] private Transform turnPoint = null;
        [SerializeField] private Vector3 oldRot = Vector3.zero;
        #endregion

        #region Build In States
        private void OnValidate()
        {
            if (moveOrigin != null)
            {
                if (agent == null)
                    agent = moveOrigin.GetComponent<NavMeshAgent>();
                agent.enabled = false;

                if (rb == null)
                    rb = moveOrigin.GetComponent<Rigidbody>();
            }
        }

        private void Start()
        {
            rb.useGravity = false;
        }

        private void Update()
        {
            GetInputFromSystem();
            Move();
            Turn();
        }
        #endregion

        #region Getters
        #endregion

        #region Setters
        #endregion

        #region In
        private void GetInputFromSystem()
        {
            moveDir.x = Input.GetAxis("Horizontal");
            if (moveDir.x < 0)
                moveDir.x = -1;
            else if (moveDir.x > 0)
                moveDir.x = 1;
            else
                moveDir.x = 0;

            moveDir.z = Input.GetAxis("Vertical");
            if (moveDir.y < 0)
                moveDir.y = -1;
            else if (moveDir.y > 0)
                moveDir.y = 1;
            else
                moveDir.y = 0;
        }
        #endregion

        #region Out
        #endregion

        #region Internal
        private void Move()
        {
            moveOrigin.position += moveDir.normalized * speed * Time.deltaTime;
        }

        private void Turn()
        {
            if (moveDir != Vector3.zero)
            {
                turnPoint.rotation = Quaternion.LookRotation(Vector3.Lerp(turnPoint.forward, moveDir, 5 * Time.deltaTime), transform.up);
            }
        }
        #endregion
    }
}