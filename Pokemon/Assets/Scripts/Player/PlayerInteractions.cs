#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Settings.Managers;
using Mfknudsen.World.Overworld.Interactions;
using UnityEngine;

#endregion

namespace Mfknudsen.Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        #region Values

        [SerializeField] private InteractItem focusedInteractable;

        private readonly Dictionary<InteractItem, Vector3> interactableInRange = new();

        #endregion

        #region Getters

        public Vector3 GetFocusedPosition()
        {
            return focusedInteractable == null ? Vector3.zero : focusedInteractable.GetPosition();
        }

        #endregion

        #region In

        public IEnumerator Setup()
        {
            while (InputManager.Instance == null)
                yield return null;

            InputManager.Instance.interactInputEvent.AddListener(TriggerClosest);
        }

        public void OnEnter(InteractItem interactable, Transform trans)
        {
            if (interactableInRange.ContainsKey(interactable)) return;

            interactableInRange.Add(interactable, trans.position);

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
            if (focusedInteractable == null) return;

            focusedInteractable.Trigger();
        }

        private void Evaluate()
        {
            if (focusedInteractable != null)
            {
                if (!interactableInRange.ContainsKey(focusedInteractable))
                    focusedInteractable = null;
            }

            Vector3 playerPos = transform.position;

            float dist = focusedInteractable == null
                ? Mathf.Infinity
                : Vector3.Distance(playerPos, focusedInteractable.GetPosition());

            foreach (InteractItem interactable in interactableInRange.Keys)
            {
                if (focusedInteractable == interactable) continue;

                float tempDist = Vector3.Distance(playerPos, interactableInRange[interactable]);

                if (!(tempDist < dist)) continue;

                dist = tempDist;
                focusedInteractable = interactable;
            }
        }

        #endregion
    }
}