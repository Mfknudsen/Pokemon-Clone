#region Packages

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Systems.Pooling
{
    public class PoolHolder
    {
        #region Values

        private readonly GameObject prefab;
        private readonly Stack<GameObject> freeObjects = new();
        private readonly HashSet<int> usedObjects = new();

        #endregion

        #region Build In States

        public PoolHolder(GameObject prefab) =>
            this.prefab = prefab;

        #endregion

        #region Out

        public GameObject GetOrCreate()
        {
            GameObject instance;

            if (this.freeObjects.Count == 0)
            {
                instance = Object.Instantiate(this.prefab);
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