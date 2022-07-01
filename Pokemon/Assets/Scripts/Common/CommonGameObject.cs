#region Packages

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mfknudsen.Common
{
    public static class CommonGameObject
    {
        public static T[] GetAllComponentsByRoot<T>(GameObject gameObject) where T : Component
        {
            List<T> result = new();

            if (gameObject.GetComponent<T>() is { } component)
                result.Add(component);

            foreach (Transform child in gameObject.transform)
                result.AddRange(GetAllComponentsByRoot<T>(child.gameObject));

            return result.Where(i => i != null).ToArray();
        }
        
        
        public static T[] GetAllMonoBehavioursByRoot<T>(GameObject gameObject) where T : MonoBehaviour
        {
            List<T> result = new();

            if (gameObject.GetComponent<T>() is { } monoBehaviour)
                result.Add(monoBehaviour);

            foreach (Transform child in gameObject.transform)
                result.AddRange(GetAllComponentsByRoot<T>(child.gameObject));

            return result.Where(i => i != null).ToArray();
        }

        public static T GetFirstComponentTowardsRoot<T>(GameObject gameObject) where T : MonoBehaviour
        {
            if (gameObject.transform.parent == null)
                return null;

            T component = gameObject.GetComponent<T>();

            if (component != null)
                return component;

            return GetFirstComponentTowardsRoot<T>(gameObject.transform.parent.gameObject);
        }
    }
}