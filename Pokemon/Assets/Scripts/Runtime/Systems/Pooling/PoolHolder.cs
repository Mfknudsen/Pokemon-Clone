#region Libraries

using Runtime.Common;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Systems.Pooling
{
    public sealed class PoolHolder
    {
        #region Values

        private object prefab;

        private readonly Stack<GameObject> freeObjects = new();

        private readonly HashSet<int> usedObjects = new();

        #endregion

        #region Setters

        public void SetPrefab<T>(T prefab)
            where T : MonoBehaviour =>
            this.prefab = prefab;

        public void SetPrefab(GameObject prefab) => this.prefab = prefab;

        #endregion

        #region Out

        public GameObject GetOrCreate()
        {
            GameObject instance = null;

            if (this.freeObjects.Count == 0)
            {
                if (this.prefab is MonoBehaviour monoBhaviour)
                    instance = Object.Instantiate(monoBhaviour.gameObject);
                else if (this.prefab is GameObject gameObject)
                    instance = Object.Instantiate(gameObject);

#if UNITY_EDITOR
                if (instance.IsNull())
                {
                    Debug.LogError("Instance was null for: " + this.prefab.ToString());
                    return null;
                }
#endif

                PoolItem item = instance.AddComponent<PoolItem>();
                item.pool = this;
            }
            else
                instance = this.freeObjects.Pop();

            this.usedObjects.Add(instance.GetInstanceID());
            return instance;
        }

        public void Free(GameObject instance)
        {
            if (this.usedObjects.Remove(instance.GetInstanceID()))
                this.freeObjects.Push(instance);
        }

        #endregion
    }
}