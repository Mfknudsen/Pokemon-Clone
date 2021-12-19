using System;
using UnityEngine;

namespace Mfknudsen.Common
{
    public class ObjectFollowTransform : MonoBehaviour
    {
        #region Values

        [SerializeField] private bool asParent;
        [SerializeField] private Transform transformToFollow;

        private Vector3 pos, rot;

        #endregion

        #region Build In States

        private void Awake()
        {
            Transform t = transform;
            if (asParent)
            {
                pos = t.position - transformToFollow.position;
                rot = t.rotation.eulerAngles - transformToFollow.rotation.eulerAngles;
            }
            else
            {
                
            }
        }

        private void Update()
        {
            Transform t = transform;

            if (asParent)
            {
                t.position = transformToFollow.position + pos;
                t.rotation = transformToFollow.rotation * Quaternion.Euler(rot);
            }
            else
            {
            }
        }

        #endregion
    }
}