#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen
{
    public class RayHitHolder : MonoBehaviour
    {
        #region Values

        [FoldoutGroup("Contains")] [SerializeField]
        private readonly bool hasComponents, hasTags;

        [FoldoutGroup("Contains"), ShowIf("hasTags")] [SerializeField]
        private readonly string[] tags;

        private readonly List<MonoBehaviour> allComponents = new List<MonoBehaviour>();

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

        public MonoBehaviour[] GetComponents()
        {
            return allComponents.ToArray();
        }

        public T[] GetComponents<T>()
            where T : MonoBehaviour
        {
            return allComponents.Where(component => component is T).Select(component => component as T).ToArray();
        }

        #endregion
    }
}