#region Packages

using Runtime.Items;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Objects.Items
{
    [CreateAssetMenu(menuName = "Variables/Item")]
    public sealed class ItemVariable : ObjectVariable<Item>
    {
        #region Values

        [SerializeField] private bool mostBeHoldable, mostBeThrowable;

        #endregion

        #region Getters

        public GameObject GetVisual() => this.value.GetVisualPrefab();

        #endregion

        #region Internal

        protected override bool ValueAcceptable(Item value)
        {
            if (this.mostBeHoldable && value is not IHoldableItem)
                return false;

            return !this.mostBeThrowable || value is IThrowableItem;
        }

        #endregion
    }
}