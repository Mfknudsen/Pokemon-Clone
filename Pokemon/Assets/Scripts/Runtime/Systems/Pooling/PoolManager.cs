#region Packages

using Runtime.Common;
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

        public static T Create<T>(T prefab, Transform parent = null, bool activate = false)
            where T : MonoBehaviour =>
            CreatePoolItem(prefab, activate, parent).GetComponent<T>();

        public static GameObject Create(GameObject prefab, Transform parent = null, bool activate = false) =>
            CreatePoolItem(prefab, activate, parent);

        public static T CreateAtPositionAndRotation<T>(T prefab, Vector3 pos, Quaternion rot,
            Transform parent = null, bool activate = false)
            where T : MonoBehaviour
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.SetPositionAndRotation(pos, rot);
            return instance.GetComponent<T>();
        }

        public static GameObject CreateAtPositionAndRotation(GameObject prefab, Vector3 pos, Quaternion rot,
            Transform parent = null, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.SetPositionAndRotation(pos, rot);
            return instance;
        }

        public static T CreateAtTransform<T>(T prefab, Transform sourceTransform,
            Transform parent = null, bool activate = false)
            where T : MonoBehaviour
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);
            return instance.GetComponent<T>();
        }

        public static GameObject CreateAtTransform(GameObject prefab, Transform sourceTransform,
            Transform parent = null, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);
            return instance;
        }

        public static T CreateAsChild<T>(T prefab, Transform parent, bool activate = false)
            where T : MonoBehaviour
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.parent = parent;
            t.SetPositionAndRotation(parent.position, parent.rotation);
            return instance.GetComponent<T>();
        }

        public static GameObject CreateAsChild(GameObject prefab, Transform parent, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.parent = parent;
            t.SetPositionAndRotation(parent.position, parent.rotation);
            return instance;
        }

        #endregion

        #region Internal

        private static GameObject CreatePoolItem<T>(T prefab, bool activateObject, Transform parent = null)
            where T : MonoBehaviour
        {
            string key = prefab.name;
            if (!Pools.TryGetValue(key, out PoolHolder pool))
            {
                pool = new PoolHolder();
                pool.SetPrefab(prefab);
                Pools.Add(key, pool);
            }

            GameObject instance = pool.GetOrCreate();

            if (!parent.IsNull())
                instance.transform.SetParent(parent, false);

            if (activateObject)
                instance.SetActive(true);

            return instance;
        }

        private static GameObject CreatePoolItem(GameObject prefab, bool activateObject, Transform parent = null)
        {
            string key = prefab.name;
            if (!Pools.TryGetValue(key, out PoolHolder pool))
            {
                pool = new PoolHolder();
                pool.SetPrefab(prefab);
                Pools.Add(key, pool);
            }

            GameObject instance = pool.GetOrCreate();

            if (!parent.IsNull())
                instance.transform.SetParent(parent, false);

            if (activateObject)
                instance.SetActive(true);

            return instance;
        }

        #endregion
    }
}