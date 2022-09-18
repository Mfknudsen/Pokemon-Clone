#region Packages

using System.Linq;
using Runtime.Player;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Interactions
{
    public sealed class InteractItem : MonoBehaviour, IInteractable
    {
        #region Values

        [SerializeField, Required] private PlayerManager playerManager;

        [FoldoutGroup(" ")] [SerializeField] private Vector3 iconOffset;

        [FoldoutGroup(" ")] [SerializeField] private MonoBehaviour[] onTrigger;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            onTrigger = onTrigger
                .OfType<IInteractable>()
                .Where(i =>
                    !(i is InteractItem))
                .Select(i =>
                    i as MonoBehaviour)
                .ToArray();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Equals("Player")) return;

            playerManager.GetInteractions().OnEnter(this, transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.tag.Equals("Player")) return;

            playerManager.GetInteractions().OnExit(this);
        }

        #endregion

        #region Getters

        public Vector3 GetPosition()
        {
            return transform.position + iconOffset;
        }

        #endregion

        #region In

        public void Trigger()
        {
            onTrigger
                .Select(script =>
                    script as IInteractable)
                .ForEach(i =>
                    i.Trigger());
        }

        #endregion
    }
}