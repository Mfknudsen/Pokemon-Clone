using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Mfknudsen
{
    public class IKFootPlacement : MonoBehaviour
    {
        [SerializeField] private bool enable;
        [SerializeField] private Animator animator;
        [SerializeField] private float distanceToGround;
        [SerializeField] private LayerMask environmentMask;

        private static readonly int HashLeftWeight = Animator.StringToHash("IKLeftFootWeight"),
            HashRightWeight = Animator.StringToHash("IKRightFootWeight");

        private void Awake()
        {
            animator ??= GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!enable || !animator || layerIndex != 0) return;

            Vector3 modelForward = transform.forward;
            float leftWeight = animator.GetFloat(HashLeftWeight),
                rightWeight = animator.GetFloat(HashRightWeight);

            SetWeight(
                AvatarIKGoal.LeftFoot,
                leftWeight);
            SetWeight(
                AvatarIKGoal.RightFoot,
                rightWeight);

            GroundFoot(
                AvatarIKGoal.LeftFoot,
                new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down),
                modelForward,
                leftWeight);
            GroundFoot(
                AvatarIKGoal.RightFoot,
                new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down),
                modelForward,
                rightWeight);
        }


        #region Internal

        private void SetWeight(AvatarIKGoal ikGoal, float weight)
        {
            animator.SetIKPositionWeight(ikGoal, weight);
            animator.SetIKRotationWeight(ikGoal, weight);
        }

        private void GroundFoot(AvatarIKGoal ikGoal, Ray ray, Vector3 modelForward, float weight)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, distanceToGround * 2 + 1f, environmentMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPos = hit.point;
                    footPos.y += distanceToGround;

                    animator.SetIKPosition(ikGoal, footPos);
                    animator.SetIKRotation(ikGoal, quaternion.LookRotation(modelForward, hit.normal));
                }
            }
        }

        #endregion
    }
}