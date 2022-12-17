#region Packages

using System.Collections.Generic;
using Runtime.ScriptableVariables.Structs.ListVariables;
using Runtime.Systems;
using Runtime.World.Overworld.Interactions;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        #region Values

        [SerializeField] private InteractItem focusedInteractable;

        [SerializeField, Required] private InteractItemListVariable itemListVariable;

        private readonly Dictionary<InteractItem, Vector3> interactableInRange = new();

        #endregion

        #region Build In States

        private void OnEnable() =>
            InputManager.instance.interactInputEvent.AddListener(this.TriggerClosest);

        private void OnDisable() =>
            InputManager.instance.interactInputEvent.RemoveListener(this.TriggerClosest);

        private void Update()
        {
            Vector3 pos = this.transform.position;
            float curSqrDistance = this.focusedInteractable != null
                ? (pos - this.focusedInteractable.GetPosition()).sqrMagnitude
                : Mathf.Infinity;
            InteractItem toSwitchTo = null;

            foreach (InteractItem interactItem in this.itemListVariable.value)
            {
                if (interactItem == this.focusedInteractable) continue;

                float sqrDist = (pos - interactItem.GetPosition()).sqrMagnitude;

                if (sqrDist > curSqrDistance) continue;

                curSqrDistance = sqrDist;

                toSwitchTo = this.focusedInteractable;
            }

            if (this.focusedInteractable != null)
            {
            }

            this.focusedInteractable = toSwitchTo;
        }

        #endregion

        #region Getters

        public Vector3 GetFocusedPosition() => 
            this.focusedInteractable == null ? Vector3.zero : this.focusedInteractable.GetPosition();

        #endregion

        #region In

        public void OnEnter(InteractItem interactable, Transform trans)
        {
            if (this.interactableInRange.ContainsKey(interactable)) return;

            this.interactableInRange.Add(interactable, trans.position);

            this.Evaluate();
        }

        public void OnExit(InteractItem interactable)
        {
            if (!this.interactableInRange.ContainsKey(interactable)) return;

            this.interactableInRange.Remove(interactable);

            this.Evaluate();
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

            Vector3 playerPos = this.transform.position;

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