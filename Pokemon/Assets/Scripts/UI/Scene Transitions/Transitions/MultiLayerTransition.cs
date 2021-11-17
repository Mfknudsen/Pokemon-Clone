#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Scene_Transitions.Transitions
{
    [CreateAssetMenu(menuName = "UI/Transition/Multi-Layer")]
    public class MultiLayerTransition : Transition
    {
        #region Values

        [SerializeField] private TransitionLayer[] transitionLayers;

        private readonly List<GameObject> objects = new List<GameObject>();

        #endregion

        public override IEnumerator Operation()
        {
            OperationsContainer container = new OperationsContainer();

            foreach (TransitionLayer transitionLayer in transitionLayers)
            {
                objects.Add(SpawnGameObject(transitionLayer.objectToAnimate));

                transitionLayer.Trigger(toCover);

                container.Add(transitionLayer);
            }

            OperationManager.instance.InsertFront(container);

            yield break;
        }

        public override void End()
        {
            foreach (GameObject gameObject in objects)
                Destroy(gameObject);
        }
    }

    [Serializable]
    internal struct TransitionLayer : IOperation
    {
        #region Values

        [SerializeField] public GameObject objectToAnimate;

        [SerializeField] private float timeFromStart;
        [SerializeField] private Animator animator;

        private bool toCover;
        private bool done;

        #endregion

        #region In

        // ReSharper disable once ParameterHidesMember
        public void Trigger(bool toCover)
        {
            this.toCover = toCover;
            done = false;
        }
        
        public IEnumerator Operation()
        {
            yield return new WaitForSeconds(timeFromStart);

            string toPlay = toCover ? "Start" : "End";
            animator.SetTrigger(toPlay);

            yield return new WaitForSeconds(Transition.GetTimeOfClipByName(animator, toPlay));

            done = true;
        }

        public bool Done()
        {
            return done;
        }

        public void End()
        {
        }

        #endregion
    }
}