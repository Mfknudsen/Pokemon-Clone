#region Packages

using System.Linq;
using Mfknudsen.Player;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.Interactions
{
    public sealed class InteractItem : MonoBehaviour, IInteractable
    {
        #region Values

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
            Debug.Log(other.tag);
            if (!other.tag.Equals("Player")) return;

            PlayerManager.instance.GetInteractions().OnEnter(this, transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.tag.Equals("Player")) return;

            PlayerManager.instance.GetInteractions().OnExit(this);
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