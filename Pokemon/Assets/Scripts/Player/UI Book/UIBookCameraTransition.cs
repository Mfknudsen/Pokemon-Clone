#region Packages

using System.Collections;
using Cinemachine;
using Mfknudsen.Battle.Systems;
using UnityEngine;

#endregion

namespace Mfknudsen.Player.UI_Book
{
    [ExecuteInEditMode]
    public class UIBookCameraTransition : MonoBehaviour, IOperation
    {
        #region Values

        public bool D;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private Transform toMove;
        [SerializeField] private Transform start, middle, end;
        [SerializeField] private float moveSpeed = 1;

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
            if (!D) return;

            Vector3 oldPos = start.position;
            float f = 0;
            while (f < 1.05f)
            {
                Vector3 newPos = LerpPosition(f, start.position, middle.position, end.position);

                Debug.DrawLine(oldPos, newPos);

                oldPos = newPos;
                f += 0.05f;
            }
        }

        #endregion

        #region Getters

        public float GetTime()
        {
            return t;
        }

        #endregion

        #region In

        public void InvertDirection(bool awayFromBook, bool? resetTime = false)
        {
            if ((awayFromBook && moveSpeed < 0) || (!awayFromBook && moveSpeed > 0))
                moveSpeed *= -1;

            if (resetTime != null && resetTime.Value)
                t = moveSpeed > 0 ? 0 : 1;
        }

        public void End()
        {
            cinemachineVirtualCamera.enabled = moveSpeed < 0;
        }

        #endregion

        #region Out

        public IEnumerator Operation()
        {
            done = false;

            while (t < 1 && moveSpeed > 0 || t > 0 && moveSpeed < 0)
            {
                toMove.position = LerpPosition(t, start.position, middle.position, end.position);
                t += moveSpeed * Time.deltaTime;
                yield return null;
            }

            done = true;
        }

        public bool Done()
        {
            return done;
        }

        #endregion

        #region Internal

        private Vector3 LerpPosition(float time, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float curveTime = curve.Evaluate(time);

            float u = 1 - curveTime;
            float tSquared = curveTime * curveTime;
            float uSquared = u * u;
            Vector3 result = uSquared * p0;
            result += 2 * u * curveTime * p1;
            result += tSquared * p2;

            return result;
        }

        #endregion
    }
}