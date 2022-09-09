#region Packages

using Cinemachine;
using Runtime.Common;
using Runtime.Player.Camera;
using Runtime.ScriptableVariables;
using Runtime.ScriptableVariables.Objects.Cinemachine;
using Runtime.ScriptableVariables.Structs;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Player
{
    public class ItemThrower : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private CinemachineVirtualCameraBase cameraRig;
        [SerializeField, Required] private BoolVariable aiming, throwing, allowed;
        [SerializeField, Required] private ItemVariable toThrow;
        [SerializeField, Required] private CinemachineFreeLookVariable defaultOverworldRig;
        [SerializeField, Required] private CinemachineBrainVariable cameraBrain;

        [HorizontalGroup("Rotation Speed")] [SerializeField]
        private float xSpeed, ySpeed;

        //Cam
        [SerializeField] private Vector2 top, mid, bot;
        [SerializeField] private AnimationCurve lerpCurve;

        [SerializeField] private float current;

        #endregion

        private void OnValidate() => MoveCam(this.current);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (float i = 0.05f; i <= 1; i += .05f)
                Gizmos.DrawLine(GizmoGetLine(i), GizmoGetLine(i - .05f));

            try
            {
                Gizmos.color = Color.white;
                Transform playerTransform = transform.parent;
                Vector3 playerPos = playerTransform.position;
                Gizmos.DrawLine(playerPos, playerPos + (playerPos - this.cameraBrain.value.transform.position));
            }
            catch
            {
                // ignored
            }
        }

        private Vector3 GizmoGetLine(float i)
        {
            if (!PlayerManager.instance) return Vector3.zero;

            float l = this.lerpCurve.Evaluate(i);

            Transform t = PlayerManager.instance.GetController().GetVisual().transform;

            return t.position
                   + Vector3.up * .4f + Vector3.right * .25f
                   - t.forward * (i < .5f
                       ? Mathf.Lerp(this.bot.y, this.mid.y, l)
                       : Mathf.Lerp(this.mid.y, this.top.y, 1f - l))
                   + Vector3.up * (i < .5f
                       ? Mathf.Lerp(this.bot.x, this.mid.x, i * 2f)
                       : Mathf.Lerp(this.mid.x, this.top.x, (i - .5f) * 2f));
        }

        #region Build In States

        private void OnEnable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.AddListener(input => this.aiming.value = input);
            inputManager.leftClickEvent.AddListener(_ => ThrowItem());
            inputManager.turnAxisInputEvent.AddListener(input => MoveCam(input.y));

            aiming.AddListener(SwitchToAiming);
        }

        private void OnDisable()
        {
            InputManager inputManager = InputManager.instance;
            inputManager.rightClickEvent.RemoveListener(input => this.aiming.value = input);
            inputManager.leftClickEvent.RemoveListener(_ => ThrowItem());
            inputManager.turnAxisInputEvent.RemoveListener(input => MoveCam(input.y));

            aiming.RemoveListener(SwitchToAiming);
        }

        #endregion

        #region Internal

        private void MoveCam(float input)
        {
            this.current -= input * this.ySpeed * Time.deltaTime;
            this.current.Clamp(0f, 1f);

            //throwAtTransform.localPosition = new Vector3(0, positionCurve.Evaluate(current), 0);
            Cinemachine3rdPersonFollow body = ((CinemachineVirtualCamera)this.cameraRig)
                .GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            float lerpCurrent = this.lerpCurve.Evaluate(this.current);

            body.CameraDistance = current < .5f
                ? Mathf.Lerp(this.bot.y, this.mid.y, lerpCurrent)
                : Mathf.Lerp(this.mid.y, this.top.y, 1f - lerpCurrent);

            body.ShoulderOffset = new Vector3(
                .25f,
                this.current < .5f
                    ? Mathf.Lerp(this.bot.x, this.mid.x, this.current * 2f)
                    : Mathf.Lerp(this.mid.x, this.top.x, (this.current - .5f) * 2f),
                0f);
        }

        private void ThrowItem()
        {
            if (this.toThrow.Empty()) return;

            this.throwing.value = true;
        }

        private void SwitchToAiming()
        {
            if (!this.allowed.value) return;

            CameraEvent cameraEvent;

            if (this.aiming.value)
            {
                this.current = this.defaultOverworldRig.value.m_YAxis.Value;
                MoveCam(this.current);

                cameraEvent = new CameraEvent(
                    this.cameraRig,
                    CameraSettings.Default(),
                    .5f,
                    .5f);
            }
            else
            {
                this.defaultOverworldRig.value.m_YAxis.Value = this.current;

                cameraEvent = new CameraEvent(
                    PlayerManager.instance.GetOverworldCameraRig(),
                    CameraSettings.Default(),
                    .5f,
                    .5f);
            }

            OperationManager.instance.AddAsyncOperationsContainer(new OperationsContainer(cameraEvent));
        }

        #endregion
    }
}