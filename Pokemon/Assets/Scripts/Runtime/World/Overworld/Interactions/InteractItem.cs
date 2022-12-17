#region Packages

using System.Linq;
using Runtime.ScriptableVariables.Structs.ListVariables;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Interactions
{
    public sealed class InteractItem : MonoBehaviour, IInteractable
    {
        #region Values

        [SerializeField, Required] private InteractItemListVariable itemListVariable;

        [SerializeField] private Vector3 iconOffset, triggerOffset;

        [SerializeField] private MonoBehaviour[] onTrigger;

        [SerializeField] private float radius;

        #endregion

        #region Build In States

#if UNITY_EDITOR

        private void OnDrawGizmosSelected() =>
            Gizmos.DrawWireSphere(this.transform.position + this.triggerOffset, this.radius);

        private void OnValidate()
        {
            this.onTrigger = this.onTrigger
                .OfType<IInteractable>()
                .Select(i => i as MonoBehaviour)
                .ToArray();
        }

#endif

        private void OnEnable() => this.itemListVariable.AddElement(this);

        private void OnDisable() => this.itemListVariable.RemoveElement(this);

        #endregion

        #region Getters

        public Vector3 GetPosition() =>
            this.transform.position + this.triggerOffset;

        public Vector3 GetIconPosition() => this.transform.position + this.iconOffset;

        public float GetRadius() => this.radius;

        #endregion

        #region In

        public void Trigger()
        {
            this.onTrigger
                .Select(script =>
                    script as IInteractable)
                .ForEach(i =>
                    i.Trigger());
        }

        #endregion
    }
}