#region Libraries

using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Systems.Pooling
{
    public struct PoolHolder
    {
        #region Values

        private Object prefab;

        private readonly Stack<PoolItem> freeObjects;
        private readonly HashSet<int> usedObjects;
        private readonly Dictionary<int, int> snapCount;

        private int currentMinimum;

        #endregion

        #region Build In States

        public PoolHolder(Object prefab)
        {
            this.prefab = prefab;

            this.freeObjects = new Stack<PoolItem>();
            this.usedObjects = new HashSet<int>();
            this.snapCount = new();

            this.currentMinimum = 1;
        }

        #endregion

        #region Getters

        private int TotalObjects =>
            this.freeObjects.Count + this.usedObjects.Count;

        #endregion

        #region In

        public void AddSnapCount(int hash, int count)
        {
            this.snapCount.TryAdd(hash, count);

            int min = this.currentMinimum;
            this.snapCount.Values.ForEach(v =>
            {
                if (v > min)
                    min = v;
            });

            this.CheckMinimumInstances();
        }

        public void RemoveSnapCount(int hash) =>
            this.snapCount.Remove(hash);

        #endregion

        #region Out

        public Object GetOrCreate()
        {
            Object instance = null;

            if (this.freeObjects.Count == 0)
            {
                instance = this.Create();
#if UNITY_EDITOR
                if (instance == null)
                {
                    Debug.LogError("Instance was null for: " + this.prefab.ToString());
                    return null;
                }
#endif
                PoolItem item = null;
                if (instance is GameObject gameObject)
                    item = gameObject.AddComponent<PoolItem>();
                else if (instance is MonoBehaviour monoBehaviour)
                    item = monoBehaviour.gameObject.AddComponent<PoolItem>();

                item.pool = this;
            }
            else
                instance = this.freeObjects.Pop().gameObject;

            this.usedObjects.Add(instance.GetInstanceID());
            return instance;
        }

        public void Free(PoolItem instance)
        {
            if (this.usedObjects.Remove(instance.GetInstanceID()))
                this.freeObjects.Push(instance);

            int diff = this.TotalObjects - this.currentMinimum;

            if (diff > 0)
            {
                List<PoolItem> list = new();
                for (int i = 0; i < diff; i++)
                {
                    if (this.freeObjects.Count == 0)
                        break;

                    list.Add(this.freeObjects.Pop());
                }

                for (int i = list.Count - 1; i >= 0; i--)
                    list[i].Destroy();
            }
        }

        #endregion

        #region Internal

        private Object Create() =>
            Object.Instantiate(this.prefab);

        private void CheckMinimumInstances()
        {
            int totalCurrent = this.TotalObjects;

            if (this.currentMinimum - totalCurrent <= 0)
                return;

            for (int i = 0; i < this.currentMinimum - totalCurrent; i++)
            {
                Object instance = this.Create();

                if (instance is GameObject gameObject)
                    this.freeObjects.Push(gameObject.GetComponent<PoolItem>());
                else if (instance is MonoBehaviour monoBehaviour)
                    this.freeObjects.Push(monoBehaviour.gameObject.GetComponent<PoolItem>());
            }
        }

        #endregion
    }
}