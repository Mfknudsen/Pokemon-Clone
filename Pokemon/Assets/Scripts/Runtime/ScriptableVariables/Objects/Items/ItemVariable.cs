#region Libraries

using Runtime.Items;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Objects.Items
{
    [CreateAssetMenu(menuName = "Variables/Item")]
    public sealed class ItemVariable : ObjectVariable<Item>
    {
        #region Values

        [SerializeField, BoxGroup("Rules")] private bool mostBeHoldable, mostBeThrowable;

        #endregion

        #region Getters

        public GameObject GetVisual() => this.Value.GetVisualPrefab();

        #endregion

        #region Internal

        protected override bool ValueAcceptable(Item item)
        {
            if (this.mostBeHoldable && item is not IHoldableItem)
            {
                if (this.debugSetter)
                    Debug.Log("Item " + item.name + " is not holdable", this);

                return false;
            }

            if (!this.mostBeThrowable || item is IThrowableItem) return true;

            if (this.debugSetter)
                Debug.Log("Item " + item.name + " is not throwable", this);

            return false;
        }

        #endregion
    }
}