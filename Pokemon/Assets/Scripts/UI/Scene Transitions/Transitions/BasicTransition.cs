#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using UnityEngine;

// ReSharper disable Unity.PreferAddressByIdToGraphicsParams

#endregion

namespace Mfknudsen.UI.Scene_Transitions.Transitions
{
    [CreateAssetMenu(menuName = "UI/Transition/Basic")]
    public class BasicTransition : Transition
    {
        #region Values

        [SerializeField] private Animator animator;
        [SerializeField] private GameObject gameObjectToAnimate;

        private GameObject instantiatedGameObject;

        #endregion

        public override IEnumerator Trigger(bool start)
        {
            if (start)
            {
                animator = transitionUI.InstantiateObject(gameObjectToAnimate).GetComponent<Animator>();
                instantiatedGameObject = animator.gameObject;
            }
            else
                onHide?.Invoke();

            SetAnimationBools(animator, start);

            //Let Unity Update so we get the clip we want
            yield return null;

            yield return new WaitForSeconds(GetTimeOfClipByName(animator));

            if (!start)
                Destroy(instantiatedGameObject);
        }
    }
}