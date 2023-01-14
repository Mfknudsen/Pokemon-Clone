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

        public void InteractTrigger()
        {
            this.playerManager.DisablePlayerControl();

            this.StartCoroutine(this.GoThroughDoor());
        }

        #endregion

        #region Internal

        private IEnumerator GoThroughDoor()
        {
            AnimationClip toPlay = this.bothWays
                    ? this.frontAnim
                    : Vector3.Angle(this.transform.position, this.playerManager.GetController().transform.position) <= 90
                        ? this.frontAnim
                        : this.backAnim;

            this.playerManager.PlayAnimationClip(toPlay);
            this.anim.Play(toPlay.name);

            yield return new WaitForSeconds(toPlay.length);

            this.playerManager.EnablePlayerControl();
        }

        #endregion
    }
}