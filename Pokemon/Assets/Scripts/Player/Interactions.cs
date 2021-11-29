#region Packages

using System.Collections.Generic;
using Mfknudsen.Settings.Manager;
using Mfknudsen.World.Overworld.Interactions;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Player
{
    public class Interactions : MonoBehaviour
    {
        #region Values

        private InteractItem focusedInteractable;

        private readonly Dictionary<InteractItem, Transform> interactableInRange =
            new Dictionary<InteractItem, Transform>();

        #endregion

        #region Getters

        public Vector3 GetFocusedPosition()
        {
            return focusedInteractable == null ? Vector3.zero : focusedInteractable.GetPosition();
        }

        #endregion

        #region In

        public void Setup()
        {
            InputManager.instance.interactInputEvent.AddListener(TriggerClosest);
        }

        public void OnEnter(InteractItem interactable, Transform transform)
        {
            if (interactableInRange.ContainsKey(interactable)) return;

            interactableInRange.Add(interactable, transform);

            Evaluate();
        }

        public void OnExit(InteractItem interactable)
        {
            if (!interactableInRange.ContainsKey(interactable)) return;

            interactableInRange.Remove(interactable);

            Evaluate();
        }

        #endregion

        #region Internal

        private void TriggerClosest()
        {
            if(focusedInteractable == null) return;
            
            focusedInteractable.Trigger();
        }

        private void Evaluate()
        {
            if (focusedInteractable != null)
            {
                if (!interactableInRange.ContainsKey(focusedInteractable))
                {
                    focusedInteractable = null;
                }
            }

            Vector3 playerPos = transform.position;

            // ReSharper disable once PossibleNullReferenceException
            float dist = focusedInteractable == null
                ? Mathf.Infinity
                : Vector3.Distance(playerPos, focusedInteractable.GetPosition());

            foreach (InteractItem interactable in interactableInRange.Keys)
            {
                if (focusedInteractable == interactable) continue;

                float tempDist = Vector3.Distance(playerPos, interactableInRange[interactable].position);

                if (!(tempDist < dist)) continue;

                dist = tempDist;
                focusedInteractable = interactable;
            }
        }

        #endregion
    }
}