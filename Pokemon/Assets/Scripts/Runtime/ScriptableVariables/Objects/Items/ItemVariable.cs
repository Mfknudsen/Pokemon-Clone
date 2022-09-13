#region Packages

using Runtime.Items;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Objects.Items
{
    [CreateAssetMenu(menuName = "Variables/Item")]
    public sealed class ItemVariable : ScriptableVariable<Item>
    {
        #region Values

        [BoxGroup("Rules")] [SerializeField] private bool mostBeHoldable, mostBeThrowable;

        #endregion

        #region Getters

        public GameObject GetVisual() => this.value.GetVisualPrefab();

        #endregion

        #region Internal

        protected override bool ValueAcceptable(Item item)
        {
            if (this.mostBeHoldable && item is not IHoldableItem)
            {
                if (this.debugSetter)
                    Debug.Log("Item is not holdable", this);

                return false;
            }

            if (!this.mostBeThrowable || item is IThrowableItem) return true;

            if (this.debugSetter)
                Debug.Log("Item is not throwable", this);

            return false;
        }

        #endregion
    }
}