#region Packages

using System.Collections;
using Cinemachine;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Common;
using UnityEngine;

#endregion

namespace Mfknudsen.Player.UI_Book
{
    [ExecuteInEditMode]
    public class UIBookCameraTransition : MonoBehaviour, IOperation
    {
        #region Values

        // ReSharper disable once InconsistentNaming
        public bool DEBUG;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private Transform toMove, endFollow;
        [SerializeField] private Transform middleLeft, middleRight;
        [SerializeField] private Transform start, end;
        [SerializeField] private float moveSpeed = 1;

        private Transform middle;
        private float t;
        private CinemachineVirtualCamera cinemachineVirtualCamera;
        private bool done;

        #endregion

        #region Build In States

        private void Awake()
        {
            cinemachineVirtualCamera = toMove.gameObject.GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (end != null && endFollow != null)
                end.position = endFollow.position;

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
            if (!DEBUG) return;

            Vector3 oldRightPos = start.position;
            Vector3 oldLeftPos = start.position;
            float f = 0;
            while (f < 1.05f)
            {
                Vector3 rightPos = ExtMathf.LerpPosition(curve, f, start.position, middleRight.position, end.position);
                Vector3 leftPos = ExtMathf.LerpPosition(curve, f, start.position, middleLeft.position, end.position);

                Debug.DrawLine(oldRightPos, rightPos);
                Debug.DrawLine(oldLeftPos, leftPos);

                oldRightPos = rightPos;
                oldLeftPos = leftPos;
                f += 0.05f;
            }
#endif
        }

        #endregion

        #region In

        public void InvertDirection(bool awayFromBook, bool? resetTime = false)
        {
            if (awayFromBook && moveSpeed < 0 || !awayFromBook && moveSpeed > 0)
                moveSpeed *= -1;

            if (resetTime != null && resetTime.Value)
                t = moveSpeed > 0 ? 0 : 1;
        }

        public void End()
        {
            cinemachineVirtualCamera.enabled = moveSpeed < 0;
        }

        public void CheckMiddle()
        {
            middle =
                Vector3.Distance(end.position, middleRight.position) <=
                Vector3.Distance(end.position, middleLeft.position)
                    ? middleRight
                    : middleLeft;
        }

        #endregion

        #region Out

        public IEnumerator Operation()
        {
            done = false;

            while (t <= 1 && t >= 0)
            {
                toMove.position = ExtMathf.LerpPosition(
                    curve,
                    t,
                    start.position,
                    middle.position,
                    end.position);
                t += moveSpeed * Time.deltaTime;
                yield return null;
            }

            done = true;
        }

        public bool Done()
        {
            return done;
        }

        public float GetTimeToComplete()
        {
            return 1 / moveSpeed;
        }

        #endregion
    }
}