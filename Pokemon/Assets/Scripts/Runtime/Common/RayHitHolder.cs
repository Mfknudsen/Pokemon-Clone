#region Packages

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Common
{
    [Serializable]
    public class RayHitHolder : Component
    {
        #region Values

        [FoldoutGroup("Contains")] [SerializeField]
        private bool hasComponents, hasTags;

        [FoldoutGroup("Contains"), ShowIf("hasTags")] [SerializeField]
        private string[] tags;

        private readonly List<MonoBehaviour> allComponents = new();

        private void OnValidate()
        {
            if (!this.hasTags) this.tags = Array.Empty<string>();
        }

        #endregion

        #region Build In States

        private void Awake()
        {
            if (this.hasComponents)
                this.allComponents.AddRange(this.gameObject.GetComponents<MonoBehaviour>());
        }

        #endregion

        #region Out

        public bool HasComponents()
        {
            return this.hasComponents;
        }

        public bool ContainsTag(string tagReference)
        {
            return this.tags.Contains(tagReference);
        }

        public bool HasTags()
        {
            return this.hasTags;
        }

        public MonoBehaviour[] GetAllComponents()
        {
            return this.allComponents.ToArray();
        }

        public T[] GetComponentsOfType<T>()
            where T : MonoBehaviour
        {
            return this.allComponents.OfType<T>().ToArray();
        }

        #endregion
    }
}