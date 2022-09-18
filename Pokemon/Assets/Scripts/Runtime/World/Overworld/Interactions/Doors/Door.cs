#region Package

using System.Collections;
using Runtime.Player;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Interactions.Doors
{
    public class Door : MonoBehaviour, IInteractable
    {
        #region Values

        [SerializeField, Required] private PlayerManager playerManager;
        
        [SerializeField] private Animator anim;
        [SerializeField] private AnimationClip frontAnim, backAnim;
        [SerializeField] private bool bothWays;

        #endregion

        #region In

        public void Trigger()
        {
            playerManager.DisablePlayerControl();

            StartCoroutine(GoThroughDoor());
        }

        #endregion

        #region Internal

        private IEnumerator GoThroughDoor()
        {
            AnimationClip toPlay =
                bothWays
                    ? frontAnim
                    : Vector3.Angle(transform.position, playerManager.GetController().transform.position) <= 90
                        ? frontAnim
                        : backAnim;

            playerManager.PlayAnimationClip(toPlay);
            anim.Play(toPlay.name);

            yield return new WaitForSeconds(toPlay.length);

            playerManager.EnablePlayerControl();
        }

        #endregion
    }
}