#region Package

using System.Collections;
using Runtime.Player;
using UnityEngine;

#endregion


namespace Runtime.World.Overworld.Interactions.Doors
{
    public class Door : MonoBehaviour, IInteractable
    {
        #region Values

        [SerializeField] private Animator anim;
        [SerializeField] private AnimationClip frontAnim, backAnim;
        [SerializeField] private bool bothWays;

        #endregion

        #region In

        public void Trigger()
        {
            PlayerManager.instance.DisablePlayerControl();

            StartCoroutine(GoThroughDoor());
        }

        #endregion

        #region Internal

        private IEnumerator GoThroughDoor()
        {
            PlayerManager pm = PlayerManager.instance;
            AnimationClip toPlay =
                bothWays
                    ? frontAnim
                    : Vector3.Angle(transform.position, pm.GetController().transform.position) <= 90
                        ? frontAnim
                        : backAnim;

            pm.PlayAnimationClip(toPlay);
            anim.Play(toPlay.name);

            yield return new WaitForSeconds(toPlay.length);

            pm.EnablePlayerControl();
        }

        #endregion
    }
}