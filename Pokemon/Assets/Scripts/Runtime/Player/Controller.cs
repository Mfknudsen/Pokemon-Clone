#region Packages

using Cinemachine;
using Runtime.ScriptableVariables.Objects.Cinemachine;
using Runtime.ScriptableVariables.Structs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Runtime.Player
{
    public sealed class Controller : MonoBehaviour
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
            visualTransform;

        [FoldoutGroup("Speeds")] [SerializeField]
        private float moveSpeed,
            rotateSpeed,
            runSpeed;

        [HorizontalGroup("Speeds/Camera")] [SerializeField]
        private float yCamSpeed, xCamSpeed;

        [FoldoutGroup("Animation")] [SerializeField]
        private float animatorDamp = 0.1f;

        [FoldoutGroup("Variables")] [SerializeField, Required]
        private BoolVariable aiming, allowed, running;

        [FoldoutGroup("Variables")] [SerializeField, Required]
        private Vec2Variable moveDirection;

        [FoldoutGroup("Variables")] [SerializeField, Required]
        private CinemachineBrainVariable cameraBrain;

        private bool ready;

        private Vector3 toLookRotation = Vector3.forward;

        private static readonly int HashWalking = Animator.StringToHash("WalkSpeed");
        
        #endregion

        #region Build In States

        private void Update()
        {
            if (!this.ready || !this.allowed.value) return;

            this.UpdateMoveTransform();
            this.Move();
            this.Turn();
            this.UpdateAnimController();
        }

        #endregion

        #region Getters

        public GameObject GetVisual() => this.visualTransform.gameObject;

        #endregion

        #region In

        public void Setup()
        {
            this.playerTransform = this.transform;

            this.agent ??= this.playerTransform.GetComponent<NavMeshAgent>();

            this.rb ??= this.playerTransform.GetComponent<Rigidbody>();
            this.rb.useGravity = false;

            this.ready = true;
        }

        public void Enable()
        {
            this.allowed.value = true;

            this.cameraRig.m_YAxis.m_MaxSpeed = this.yCamSpeed;
            this.cameraRig.m_XAxis.m_MaxSpeed = this.xCamSpeed;
        }

        public void Disable()
        {
            this.allowed.value = false;

            this.cameraRig.m_YAxis.m_MaxSpeed = 0;
            this.cameraRig.m_XAxis.m_MaxSpeed = 0;
        }

        public void TriggerAnimator(int triggerID)
        {
            this.animController.SetTrigger(triggerID);
        }

        #endregion

        #region Internal

        private void UpdateMoveTransform()
        {
            if (this.aiming.value) return;

            Vector3 camForward = this.cameraBrain.getTransform.forward;
            camForward -= Vector3.up * camForward.y;

            this.moveTransform.LookAt(this.moveTransform.position + camForward, Vector3.up);
        }

        private void UpdateAnimController()
        {
            if (!this.animController) return;

            Vector3 playerInputDirection = this.moveDirection.value;

            this.animController.SetFloat(
                HashWalking,
                playerInputDirection.magnitude * 2 * (this.running.value ? 3 : 1),
                this.animatorDamp,
                Time.deltaTime);
        }

        private void Move()
        {
            if (!this.agent || !this.agent.isOnNavMesh) return;

            Vector2 playerInputDirection = this.moveDirection.value;
            Vector3 forwardMove = this.moveTransform.forward * playerInputDirection.y;
            Vector3 sideMove = this.moveTransform.right * playerInputDirection.x;
            Vector3 moveVector = (forwardMove + sideMove).normalized;

            this.agent.Move(moveVector.normalized *
                            ((this.running.value ? this.runSpeed : this.moveSpeed) * Time.deltaTime));
        }

        private void Turn()
        {
            if (!this.aiming.value)
            {
                if (this.moveDirection.value != Vector2.zero)
                {
                    Vector2 playerInputDirection = this.moveDirection.value;
                    Vector3 forwardMove = this.moveTransform.forward * playerInputDirection.y;
                    Vector3 sideMove = this.moveTransform.right * playerInputDirection.x;
                    this.toLookRotation = (forwardMove + sideMove).normalized;
                    this.toLookRotation -= new Vector3(0, this.toLookRotation.y, 0);
                }

                this.visualTransform.rotation = Quaternion.Lerp(this.visualTransform.rotation,
                    Quaternion.LookRotation(this.toLookRotation), this.rotateSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 camForward = this.cameraBrain.getTransform.forward;
                camForward -= Vector3.up * camForward.y;
                this.toLookRotation = camForward;
            }
        }

        #endregion
    }
}