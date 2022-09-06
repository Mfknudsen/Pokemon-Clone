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

        [SerializeField] private Transform throwAtTransform;
        [SerializeField] private CinemachineVirtualCameraBase cameraRig;
        [SerializeField] private BoolVariable aiming, throwing, allowed;
        [SerializeField] private ItemVariable toThrow;
        [SerializeField] private CinemachineFreeLookVariable defaultOverworldRig;
        [SerializeField] private CinemachineBrainVariable cameraBrain;

        [HorizontalGroup("Rotation Speed")] [SerializeField]
        private float xSpeed, ySpeed;

        //Cam
        [SerializeField] private Vector2 top, mid, bot;
        [SerializeField] private AnimationCurve lerpCurve;

        private AnimationCurve positionCurve, distanceCurve;
        [SerializeField] private float current;

        #endregion

        private void OnValidate()
        {
            positionCurve = new AnimationCurve();
            positionCurve.AddKey(1f, top.x);
            positionCurve.AddKey(.5f, mid.x);
            positionCurve.AddKey(0f, bot.x);

            distanceCurve = new AnimationCurve();
            distanceCurve.AddKey(1f, top.y);
            distanceCurve.AddKey(.5f, mid.y);
            distanceCurve.AddKey(0f, bot.y);

            MoveCam(current);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (float i = 0.05f; i < 1; i += .05f)
                Gizmos.DrawLine(GizmoGetLine(i), GizmoGetLine(i - .05f));

            try
            {
                Gizmos.color = Color.white;
                Transform playerTransform = transform.parent;
                Vector3 playerPos = playerTransform.position;
                Gizmos.DrawLine(playerPos, playerPos + (playerPos - cameraBrain.value.transform.position));
            }
            catch
            {
                // ignored
            }
        }

        private Vector3 GizmoGetLine(float i)
        {
            float l = lerpCurve.Evaluate(i);

            return transform.position - (i < .5f
                       ? Vector3.forward * Mathf.Lerp(bot.y, mid.y, l)
                       : Vector3.forward * Mathf.Lerp(mid.y, top.y, 1f - l))
                   + Vector3.up * (i < .5f
                       ? Mathf.Lerp(bot.x, mid.x, l)
                       : Mathf.Lerp(mid.x, top.x, 1 - l));
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
            current += input * Time.deltaTime / 10f;
            current.Clamp(0f, 1f);

            //throwAtTransform.localPosition = new Vector3(0, positionCurve.Evaluate(current), 0);
            Cinemachine3rdPersonFollow body = ((CinemachineVirtualCamera)this.cameraRig)
                .GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            float lerpCurrent = lerpCurve.Evaluate(current);

            body.CameraDistance = current < .5f
                ? Mathf.Lerp(bot.y, mid.y, lerpCurrent)
                : Mathf.Lerp(mid.y, top.y, 1f - lerpCurrent);

            body.ShoulderOffset = new Vector3(.5f, 1, 0) * (current < .5f
                ? Mathf.Lerp(bot.x, mid.x, lerpCurrent)
                : Mathf.Lerp(mid.x, top.x, 1 - lerpCurrent));
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
                cameraEvent = new CameraEvent(
                    this.cameraRig,
                    CameraSettings.Default(),
                    .5f,
                    .5f);
            }
            else
            {
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