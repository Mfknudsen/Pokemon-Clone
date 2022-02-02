#region Packages

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.Common
{
    public class RayHitHolder : MonoBehaviour
    {
        #region Values

        [FoldoutGroup("Contains")] [SerializeField]
        private bool hasComponents, hasTags;

        [FoldoutGroup("Contains"), ShowIf("hasTags")] [SerializeField]
        private string[] tags;

        private readonly List<MonoBehaviour> allComponents = new List<MonoBehaviour>();

        private void OnValidate()
        {
            if (!hasTags)
                tags = Array.Empty<string>();
        }

        #endregion

        #region Build In States

        private void Awake()
        {
            if (hasComponents)
                allComponents.AddRange(
                    gameObject.GetComponents<MonoBehaviour>()
                        .Where(component => !(component is RayHitHolder)));
        }

        #endregion

        #region Out

        public bool HasComponents()
        {
            return hasComponents;
        }

        public bool ContainsTag(string tagReference)
        {
            return tags.Contains(tagReference);
        }

        public bool HasTags()
        {
            return hasTags;
        }

        public MonoBehaviour[] GetAllComponents()
        {
            return allComponents.ToArray();
        }

        public T[] GetComponentsOfType<T>()
            where T : MonoBehaviour
        {
            return allComponents.OfType<T>().ToArray();
        }

        #endregion
    }
}