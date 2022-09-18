#region Packages

using System;
using System.Collections;
using Cinemachine;
using Runtime.Common;
using Runtime.Systems.Operation;
using UnityEngine;

#endregion

namespace Runtime.UI_Book
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
        [SerializeField] private CinemachineBlenderSettings settings;

        private Transform middle;
        private float t;
        private CinemachineVirtualCamera cinemachineVirtualCamera;
        private bool done;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            if (settings == null || settings.m_CustomBlends == null)
                return;

            settings.m_CustomBlends[0].m_Blend.m_Time = 1 - Math.Abs(moveSpeed);
            settings.m_CustomBlends[1].m_Blend.m_Time = 1 - Math.Abs(moveSpeed);
        }

        private void Awake()
        {
            cinemachineVirtualCamera = toMove.gameObject.GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (end != null && endFollow != null)
                end.position = endFollow.position;

#if UNITY_EDITOR
            if (!DEBUG) return;

            Vector3 startPosition = start.position,
                endPosition = end.position;

            Vector3 oldRightPos = startPosition,
                oldLeftPos = startPosition;

            float floatTime = 0;

            while (floatTime <= 1)
            {
                Vector3 rightPos = ExtMathf.LerpPosition(
                    curve,
                    floatTime,
                    startPosition,
                    middleRight.position,
                    endPosition);

                Vector3 leftPos = ExtMathf.LerpPosition(
                    curve,
                    floatTime,
                    startPosition,
                    middleLeft.position,
                    endPosition);

                Debug.DrawLine(
                    oldRightPos,
                    rightPos);

                Debug.DrawLine(
                    oldLeftPos,
                    leftPos);

                oldRightPos = rightPos;
                oldLeftPos = leftPos;
                floatTime += 0.025f;
            }
#endif
        }

        #endregion

        #region Getters

        public float GetSpeed()
        {
            return moveSpeed;
        }

        #endregion

        #region In

        public void Direction(bool awayFromBook, bool? resetTime = false)
        {
            if (awayFromBook && moveSpeed < 0 ||
                !awayFromBook && moveSpeed > 0)
                moveSpeed *= -1;

            if (resetTime != null &&
                resetTime.Value)
                t = moveSpeed > 0 ? 0 : 1;
        }

        public void OperationEnd()
        {
            cinemachineVirtualCamera.enabled = moveSpeed < 0;
        }

        public void CheckMiddle()
        {
            Vector3 endPosition = end.position;
            middle =
                Vector3.Distance(endPosition, middleRight.position) <=
                Vector3.Distance(endPosition, middleLeft.position)
                    ? middleRight
                    : middleLeft;
        }

        #endregion

        #region Out

        public IEnumerator Operation()
        {
            done = false;

            while (t is <= 1 and >= 0)
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

        public bool IsOperationDone()
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