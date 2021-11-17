#region Packages

using System;
using System.Collections;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Scene_Transitions.Transitions
{
    [CreateAssetMenu(menuName = "UI/Transition/Multi-Layer")]
    public class MultiLayerTransition : Transition
    {
        #region Values

        [SerializeField] private TransitionLayer[] transitionLayers;

        #endregion

        public override IEnumerator Trigger(bool start)
        {
            foreach (TransitionLayer layer in transitionLayers)
                transitionUI.StartCoroutine(layer.Trigger(start, transitionUI));

            foreach (TransitionLayer layer in transitionLayers)
            {
                while (!layer.Done())
                    yield return null;
            }
        }
    }

    [Serializable]
    public class TransitionLayer : MonoBehaviour
    {
        #region Values

        [SerializeField] private GameObject gameObjectToAnimate;

        [SerializeField] private float timeFromStart;
        [SerializeField] private Animator animator;

        private bool done;

        #endregion

        #region In

        public IEnumerator Trigger(bool start, SceneTransitionUI transitionUI)
        {
            done = false;

            yield return new WaitForSeconds(timeFromStart);

            if (start)
                animator = transitionUI.InstantiateObject(gameObjectToAnimate).GetComponent<Animator>();

            Transition.SetAnimationBools(animator, start);

            yield return null;

            yield return new WaitForSeconds(Transition.GetTimeOfClipByName(animator));

            done = true;
        }

        public bool Done()
        {
            return done;
        }

        public void End()
        {
            Destroy(animator.gameObject);
        }

        #endregion
    }
}