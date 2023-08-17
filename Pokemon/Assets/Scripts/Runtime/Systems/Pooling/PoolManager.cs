#region Libraries

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#endregion

namespace Runtime.Systems.Pooling
{
    [InitializeOnLoad]
    public static class PoolManager
    {
        #region Values

        private static readonly Dictionary<int, PoolHolder> Pools = new Dictionary<int, PoolHolder>();

        #endregion

        #region Build In States

        static PoolManager() => EditorApplication.playModeStateChanged += Reset;

        #endregion

        #region In

        public static void AddSnapshot(int id, Object prefab, int count)
        {
            if (Pools.TryGetValue(prefab.GetHashCode(), out PoolHolder pool))
                pool.AddSnapCount(id, count);
        }

        public static void RemoveSnapshot(int id, Object prefab)
        {
            if (Pools.TryGetValue(prefab.GetHashCode(), out PoolHolder pool))
                pool.RemoveSnapCount(id);
        }

        public static GameObject Create(Object prefab, Transform parent = null, bool activate = false) =>
            CreatePoolItem(prefab, activate, parent);

        public static GameObject CreateAtPositionAndRotation(Object prefab, Vector3 pos, Quaternion rot,
            Transform parent = null, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.SetPositionAndRotation(pos, rot);
            return instance;
        }

        public static GameObject CreateAtTransform(Object prefab, Transform sourceTransform,
            Transform parent = null, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);
            return instance;
        }

        public static GameObject CreateAsChild(Object prefab, Transform parent, bool activate = false)
        {
            GameObject instance = CreatePoolItem(prefab, activate, parent);
            Transform t = instance.transform;
            t.parent = parent;
            t.SetPositionAndRotation(parent.position, parent.rotation);
            return instance;
        }

        #endregion

        #region Internal

        private static GameObject CreatePoolItem(Object prefab, bool activateObject, Transform parent = null)
        {
            int key = prefab.GetHashCode();
            if (!Pools.TryGetValue(key, out PoolHolder pool))
            {
                pool = new PoolHolder(prefab);
                Pools.Add(key, pool);
            }

            Object instance = pool.GetOrCreate();
            GameObject obj = null;

            if (instance is GameObject gameObject)
                obj = gameObject;
            else if (instance is MonoBehaviour monoBehaviour)
                obj = monoBehaviour.gameObject;

            if (parent != null)
                obj.transform.SetParent(parent, false);

            obj.SetActive(activateObject);

            return obj;
        }

#if UNITY_EDITOR
        private static void Reset(PlayModeStateChange state) => Pools.Clear();
#endif

        #endregion
    }
}