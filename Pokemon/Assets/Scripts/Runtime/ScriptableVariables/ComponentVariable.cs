#region Packages

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ComponentVariable<TGeneric> : ObjectVariable<TGeneric> where TGeneric : Component
    {
        #region Values

        [SerializeField, ReadOnly] private Transform componentTransform;

        [SerializeField, ReadOnly] private GameObject componentGameObject;

        #endregion

        #region Getters

        public Transform getTransform
        {
            get
            {
                if (this.componentTransform == null)
                    this.componentTransform = this.value.transform;

                return this.componentTransform;
            }
        }

        public GameObject getGameObject
        {
            get
            {
                if (this.componentGameObject == null)
                    this.componentGameObject = this.value.gameObject;

                return this.componentGameObject;
            }
        }

        #endregion
    }
}