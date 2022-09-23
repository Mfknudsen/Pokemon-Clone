#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Systems;
using Runtime.World.Overworld.Interactions;
using UnityEngine;

#endregion

namespace Runtime.Player
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
            return this.focusedInteractable == null ? Vector3.zero : this.focusedInteractable.GetPosition();
        }

        #endregion

        #region In

        public IEnumerator Setup()
        {
            while (InputManager.instance == null)
                yield return null;

            InputManager.instance.interactInputEvent.AddListener(TriggerClosest);
        }

        public void OnEnter(InteractItem interactable, Transform trans)
        {
            if (this.interactableInRange.ContainsKey(interactable)) return;

            this.interactableInRange.Add(interactable, trans.position);

            Evaluate();
        }

        public void OnExit(InteractItem interactable)
        {
            if (!this.interactableInRange.ContainsKey(interactable)) return;

            this.interactableInRange.Remove(interactable);

            Evaluate();
        }

        #endregion

        #region Internal

        private void TriggerClosest()
        {
            if (this.focusedInteractable == null) return;

            this.focusedInteractable.Trigger();
        }

        private void Evaluate()
        {
            if (this.focusedInteractable != null)
            {
                if (!this.interactableInRange.ContainsKey(this.focusedInteractable)) this.focusedInteractable = null;
            }

            Vector3 playerPos = transform.position;

            float dist = this.focusedInteractable == null
                ? Mathf.Infinity
                : Vector3.Distance(playerPos, this.focusedInteractable.GetPosition());

            foreach (InteractItem interactable in this.interactableInRange.Keys)
            {
                if (this.focusedInteractable == interactable) continue;

                float tempDist = Vector3.Distance(playerPos, this.interactableInRange[interactable]);

                if (!(tempDist < dist)) continue;

                dist = tempDist;
                this.focusedInteractable = interactable;
            }
        }

        #endregion
    }
}