#region Packages

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Systems.Pooling
{
    public static class PoolManager
    {
        #region Values

        private static readonly Dictionary<string, PoolHolder> Pools = new();

        #endregion

        #region In

        public static GameObject Create(GameObject prefab, Transform parent = null, bool activate = false) =>
            CreatePoolItem(prefab, activate, parent);

        public static T Create<T>(T prefab, Transform parent = null, bool activate = false) where T : MonoBehaviour =>
            CreatePoolItem(prefab.gameObject, activate, parent).GetComponent<T>();

        public static GameObject CreateAtPositionAndRotation(GameObject prefab, Vector3 pos, Quaternion rot,
            Transform parent = null, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.position = pos;
            t.rotation = rot;
            return instance;
        }

        public static GameObject CreateAtTransform(GameObject prefab, Transform sourceTransform,
            Transform parent = null, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.position = sourceTransform.position;
            t.rotation = sourceTransform.rotation;
            return instance;
        }

        public static GameObject CreateAsChild(GameObject prefab, Transform parent, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.parent = parent;
            t.position = parent.position;
            t.rotation = parent.rotation;
            return instance;
        }

        #endregion

        #region Internal

        private static GameObject CreatePoolItem(GameObject prefab, bool activateObject, Transform parent = null)
        {
            string key = prefab.name;
            if (!Pools.TryGetValue(key, out PoolHolder pool))
            {
                pool = new PoolHolder(prefab);
                Pools[key] = pool;
            }

            GameObject instance = pool.GetOrCreate();

            if (parent != null)
                instance.transform.SetParent(parent, false);

            if (activateObject)
                instance.SetActive(true);

            return instance;
        }

        #endregion
    }
}