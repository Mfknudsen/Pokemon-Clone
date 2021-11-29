#region Packages

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.Interactions
{
    public abstract class InteractItem : MonoBehaviour, IInteractable
    {
        #region Values

        [FoldoutGroup("Interact Item")] [SerializeField]
        protected Vector3 iconOffset;

        #endregion

        #region Getters

        public Vector3 GetPosition()
        {
            return transform.position + iconOffset;
        }

        #endregion

        #region In

        public abstract void Trigger();

        #endregion
    }
}