#region Packages

using System.Collections;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Scene_Transitions.Transitions
{
    [CreateAssetMenu(menuName = "UI/Transition/Basic")]
    public class BasicTransition : Transition
    {
        #region Values

        [SerializeField] private GameObject objectToAnimate;
        
        private Animator animator;
        private GameObject instantiatedObject;

        #endregion

        #region In

        public override IEnumerator Operation()
        {
            string toPlay = toCover ? "Start" : "End";

            instantiatedObject = instantiatedObject == null ? SpawnGameObject(objectToAnimate) : instantiatedObject;
            animator = instantiatedObject.GetComponent<Animator>();

            animator.SetTrigger(toPlay);

            yield return new WaitForSeconds(GetTimeOfClipByName(animator, toPlay));

            done = true;
        }

        public override void End()
        {
            if(toCover) return;
            
            Destroy(instantiatedObject);
        }

        #endregion
    }
}