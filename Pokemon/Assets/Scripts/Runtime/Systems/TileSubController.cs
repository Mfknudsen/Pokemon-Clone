#region Libraries

using Runtime.Systems.Pooling;
using Runtime.World.Overworld.Tiles;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public sealed class TileSubController : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private TileManager tileManager;

        [SerializeField] private Neighbor[] neighbors;

        [SerializeField, Required] private NavMeshSurface navMeshSurface;

        [SerializeField]
        private SnapshotItem[] poolingSnapshot;

        #endregion

        #region Build In States

        private void OnEnable()
        {
            this.tileManager.AddSubManager(this);

            this.poolingSnapshot.ForEach(item =>
                PoolManager.AddSnapshot(this.GetHashCode(), item.prefab, item.count));
        }

        private void OnDisable()
        {
            this.tileManager.RemoveSubManager(this);

            this.poolingSnapshot.ForEach(item =>
                PoolManager.RemoveSnapshot(this.GetHashCode(), item.prefab));
        }

        #endregion

        #region Getters

        public IEnumerable<Neighbor> GetNeighbors() => this.neighbors;

        public string GetTileName() => this.name;

        public NavMeshSurface GetSurface() => this.navMeshSurface;

        #endregion

        #region In

        public void EnableDividers()
        {
        }

        public void DisableDividers()
        {
        }

        public void Unload()
        {
        }

        #endregion
    }

    [Serializable]
    public class Neighbor
    {
        [SerializeField] private string sceneName;

        public string GetSceneName() => this.sceneName;
    }

    [Serializable]
    internal struct SnapshotItem
    {
        [SerializeField, Min(1f)]
        internal int count;
        [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefabs", Filter = "t:GameObject t:MonoBehaviour", IsUniqueList = false), Required]
        internal UnityEngine.Object prefab;
    }
}