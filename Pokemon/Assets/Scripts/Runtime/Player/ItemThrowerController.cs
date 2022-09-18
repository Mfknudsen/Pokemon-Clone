#region Packages

using Cinemachine;
using Runtime.Common;
using Runtime.Player.Camera;
using Runtime.ScriptableVariables.Objects.Cinemachine;
using Runtime.ScriptableVariables.Objects.Items;
using Runtime.ScriptableVariables.Structs;
using Runtime.Systems;
using Runtime.Systems.Operation;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Player
{
    public class ItemThrowerController : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private OperationManager operationManager;
        [SerializeField, Required] private PlayerManager playerManager;

        [SerializeField, Required] private CinemachineVirtualCameraBase cameraRig;
        [SerializeField, Required] private BoolVariable aiming, throwing, allowed;
        [SerializeField, Required] private ItemVariable toThrow;
        [SerializeField, Required] private CinemachineFreeLookVariable defaultOverworldRig;
        [SerializeField, Required] private CinemachineBrainVariable cameraBrain;
        [SerializeField, Required] private Vec2Variable throwRotationSpeed;
        [SerializeField, Required] private Transform throwTransform;

        //Cam
        [SerializeField] private float shoulderOffset;
        [SerializeField] private Vector2 top, mid, bot;
        [SerializeField] private AnimationCurve lerpCurve;

        [SerializeField] private float current;

        private Cinemachine3rdPersonFollow bodyComponent;
        private CinemachineComposer aimComponent;

        private OperationsContainer cameraSwitchAsyncContainer;

        private float currentShoulderOffset;

        private IOperation throwingOperation;

        private Timer throwDelay;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            this.bodyComponent = this.cameraRig.CinemachineComponent<Cinemachine3rdPersonFollow>();
            this.aimComponent = this.cameraRig.CinemachineComponent<CinemachineComposer>();

            MoveCamera();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Transform t = transform;
            for (float i = 0.05f; i <= 1; i += .05f)
            {
                float l = this.lerpCurve.Evaluate(i);
                float j = this.lerpCurve.Evaluate(i - .05f);

                Gizmos.DrawLine(
                    t.position
                    + Vector3.up * .4f + Vector3.right * this.shoulderOffset
                    - t.forward * (i < .5f
                        ? Mathf.Lerp(this.bot.y, this.mid.y, l)
                        : Mathf.Lerp(this.mid.y, this.top.y, 1f - l))
                    + Vector3.up * (i < .5f
                        ? Mathf.Lerp(this.bot.x, this.mid.x, i * 2f)
                        : Mathf.Lerp(this.mid.x, this.top.x, (i - .5f) * 2f)),
                    t.position
                    + Vector3.up * .4f + Vector3.right * this.shoulderOffset
                    - t.forward * (i - .05f < .5f
                        ? Mathf.Lerp(this.bot.y, this.mid.y, j)
                        : Mathf.Lerp(this.mid.y, this.top.y, 1f - j))
                    + Vector3.up * (i - .05f < .5f
                        ? Mathf.Lerp(this.bot.x, this.mid.x, (i - .05f) * 2f)
                        : Mathf.Lerp(this.mid.x, this.top.x, (i - .05f - .5f) * 2f)));
            }
        }

        private void OnEnable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.AddListener(input => this.aiming.value = input);
            inputManager.leftClickEvent.AddListener(_ => ThrowItem());
            inputManager.turnAxisInputEvent.AddListener(input => MoveCamera(input.y));

            this.aiming.AddListener(SwitchToAiming);

            this.bodyComponent = this.cameraRig.CinemachineComponent<Cinemachine3rdPersonFollow>();
            this.aimComponent = this.cameraRig.CinemachineComponent<CinemachineComposer>();
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.RemoveListener(input => this.aiming.value = input);
            inputManager.leftClickEvent.RemoveListener(_ => ThrowItem());
            inputManager.turnAxisInputEvent.RemoveListener(input => MoveCamera(input.y));

            this.aiming.RemoveListener(SwitchToAiming);

            this.throwDelay?.Stop();
        }

        #endregion

        #region Internal

        private void MoveCamera(float input = 0)
        {
            if (input != 0)
                this.current -= input * this.throwRotationSpeed.y * Time.deltaTime;
            this.current.RefClamp(0f, 1f);

            if (this.bodyComponent == null || this.aimComponent == null) return;

            float lerpCurrent = this.lerpCurve.Evaluate(this.current);

            this.bodyComponent.CameraDistance = current < .5f
                ? Mathf.Lerp(this.bot.y, this.mid.y, lerpCurrent)
                : Mathf.Lerp(this.mid.y, this.top.y, 1f - lerpCurrent);

            this.bodyComponent.ShoulderOffset = new Vector3(
                this.shoulderOffset,
                this.current < .5f
                    ? Mathf.Lerp(this.bot.x, this.mid.x, this.current * 2f)
                    : Mathf.Lerp(this.mid.x, this.top.x, (this.current - .5f) * 2f),
                0f);

            this.aimComponent.m_TrackedObjectOffset = Vector3.right * this.shoulderOffset;
        }

        private void ThrowItem()
        {
            if (!this.allowed.value || this.toThrow.valueEmpty || this.throwing.value || !this.aiming.value) return;

            this.throwing.value = true;

            GameObject spawnedItem = Instantiate(
                this.toThrow.GetVisual(),
                this.throwTransform.position + this.cameraBrain.getTransform.forward * .5f,
                this.cameraBrain.getTransform.rotation);

            spawnedItem.GetComponent<Rigidbody>()
                .AddForce(this.cameraBrain.getTransform.forward * 10, ForceMode.Impulse);

            this.throwDelay = new Timer(1, () =>
            {
                this.throwing.value = false;
                this.throwDelay = null;
            });

            Debug.Log("Spawned");
        }

        private void SwitchToAiming()
        {
            if (!this.allowed.value) return;

            if (this.cameraSwitchAsyncContainer != null)
                operationManager.StopAsyncContainer(this.cameraSwitchAsyncContainer);
            this.cameraSwitchAsyncContainer = new OperationsContainer();

            CameraEvent cameraEvent;

            if (this.aiming.value)
            {
                this.current = this.defaultOverworldRig.value.m_YAxis.Value;
                MoveCamera();

                cameraEvent = ScriptableObject.CreateInstance<CameraEvent>().Setup(
                    this.cameraRig,
                    CameraSettings.Default(),
                    .5f);
            }
            else
            {
                this.defaultOverworldRig.value.m_YAxis.Value = this.current;
                this.defaultOverworldRig.value.m_XAxis.Value = this.cameraBrain.getTransform.rotation.eulerAngles.y;

                cameraEvent = ScriptableObject.CreateInstance<CameraEvent>().Setup(
                    playerManager.GetOverworldCameraRig(),
                    CameraSettings.Default(),
                    .5f);
            }

            this.cameraSwitchAsyncContainer.Add(cameraEvent);
            operationManager.AddAsyncOperationsContainer(this.cameraSwitchAsyncContainer);
        }

        #endregion
    }
}