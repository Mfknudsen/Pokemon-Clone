#region Packages

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ComponentVariable<TGeneric> : ScriptableVariable<TGeneric> where TGeneric : Component
    {
        #region Values

        [ShowInInspector, ReadOnly] private Transform componentTransform;

        [ShowInInspector, ReadOnly] private GameObject componentGameObject;

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