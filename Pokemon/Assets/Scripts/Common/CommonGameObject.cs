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

            return result.Where(i => i != null)
                .ToArray();
        }

        public static T GetFirstComponentByRoot<T>(GameObject gameObject) where T : Component
        {
            if (gameObject.GetComponent<T>() is { } component)
                return component;

            List<GameObject> toCheck = (from Transform t in gameObject.transform select t.gameObject)
                .ToList();

            while (toCheck.Count > 0)
            {
                GameObject child = toCheck[0];

                if (child.GetComponent<T>() is { } childComponent)
                    return childComponent;

                toCheck.AddRange(from Transform t in child.transform select t.gameObject);

                toCheck.RemoveAt(0);
            }

            return null;
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
            while (true)
            {
                if (gameObject.transform.parent == null) return null;

                T component = gameObject.GetComponent<T>();

                if (component != null) return component;

                gameObject = gameObject.transform.parent.gameObject;
            }
        }

        public static GameObject GetChildByName(Transform parent, string name)
        {
            return (from Transform t in parent where t.name.Equals(name) select t.gameObject).FirstOrDefault();
        }
    }
}