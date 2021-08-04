#region SDK

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.World.Overworld;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Player
{
    public class Interactions : MonoBehaviour
    {
        #region Values

        private IInteractable focusedInteractable;
        private Transform focusedTransform;

        private readonly Dictionary<IInteractable, Transform> interactableInRange =
            new Dictionary<IInteractable, Transform>();

        #endregion

        #region Getters

        public Vector3 GetFocusedPosition()
        {
            return focusedTransform == null ? Vector3.zero : focusedTransform.position;
        }

        #endregion

        #region In

        public void OnInteractionTrigger(InputAction.CallbackContext value)
        {
            if (!value.performed) return;

            focusedInteractable?.Trigger();
        }

        public void OnEnter(IInteractable interactable, Transform transform)
        {
            if (interactableInRange.ContainsKey(interactable)) return;

            interactableInRange.Add(interactable, transform);

            Evaluate();
        }

        public void OnExit(IInteractable interactable)
        {
            if (!interactableInRange.ContainsKey(interactable)) return;

            interactableInRange.Remove(interactable);

            Evaluate();
        }

        #endregion

        #region Internal

        private void Evaluate()
        {
            if (focusedInteractable != null)
            {
                if (!interactableInRange.ContainsKey(focusedInteractable))
                {
                    focusedInteractable = null;
                    focusedTransform = null;
                }
            }

            Vector3 playerPos = transform.position;

            // ReSharper disable once PossibleNullReferenceException
            float dist = focusedInteractable == null
                ? Mathf.Infinity
                : Vector3.Distance(playerPos, focusedTransform.position);

            foreach (IInteractable interactable in interactableInRange.Keys)
            {
                if (focusedInteractable == interactable) continue;

                float tempDist = Vector3.Distance(playerPos, interactableInRange[interactable].position);

                if (!(tempDist < dist)) continue;

                dist = tempDist;
                focusedInteractable = interactable;
                focusedTransform = interactableInRange[interactable];
            }
        }

        #endregion
    }
}