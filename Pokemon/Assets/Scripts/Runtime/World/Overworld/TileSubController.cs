#region Libraries

using Runtime.AI.Navigation;
using Runtime.Systems.Pooling;
using Runtime.Variables;
using Runtime.World.Overworld.Tiles;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

#endregion

namespace Assets.Scripts.Runtime.World.Overworld
{
    public sealed class TileSubController : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private TileManager tileManager;

        [SerializeField] private Neighbor[] neighbors;

        [SerializeField, FoldoutGroup("Navigation"), Required] private NavMeshSurface navMeshSurface;

        [SerializeField, FoldoutGroup("Navigation")] private Vector3 navmeshCleanUpPoint;

        [SerializeField] private PoolSnapshotItem[] poolingSnapshot;

        [SerializeField] private CalculatedNavMesh calculatedNavMesh;

        #endregion

        #region Build In States

        private void OnValidate() =>
            this.name = this.gameObject.scene.name + " - TileSubController";

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

        private void OnDrawGizmos()
        {
            return;

            Vector3[] verts = this.calculatedNavMesh.Vertices();
            NavTriangle[] triangles = this.calculatedNavMesh.Triangles;
            List<NavTriangle> drawn = new();

            foreach (NavTriangle t in triangles)
            {
                Debug.DrawLine(t.Center(verts), t.Center(verts) + Vector3.up, Color.green);

                if (drawn.Contains(t))
                    continue;

                drawn.Add(t);

                NavTriangle[] toDraw = t.Neighbors.Select(i => triangles[i]).Where(t => !drawn.Contains(t)).ToArray();

                toDraw.ForEach(draw => Debug.DrawLine(draw.Center(verts) + Vector3.up, t.Center(verts) + Vector3.up, Color.green));
            }
        }

        #endregion

        #region Getters

        public IEnumerable<Neighbor> GetNeighbors() => this.neighbors;

        public string GetTileName() => this.name;

        public NavMeshSurface GetSurface() => this.navMeshSurface;

#if UNITY_EDITOR
        public CalculatedNavMesh GetNavmesh => this.calculatedNavMesh;

        public Vector3 GetCleanUpPoint => this.navmeshCleanUpPoint;
#endif

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetCalculatedNavMesh(CalculatedNavMesh set) => this.calculatedNavMesh = set;

        public void SetCleanUpPoint(Vector3 set) => this.navmeshCleanUpPoint = set;
#endif

        #endregion

        #region In

        public void SetAsCurrent()
        {
            UnitNavigation.SetNavMesh(this.calculatedNavMesh);
        }

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

        #region Out

        public NavigationPoint[] GetNavigationPoints()
        {
            return GameObject.FindObjectsByType<NavigationPoint>(FindObjectsSortMode.None)
                .OrderBy(p => p.gameObject.scene.name)
                .ThenBy(p => p.gameObject.GetInstanceID())
                .ToArray();
        }

        #endregion
    }

    [Serializable]
    public sealed class Neighbor
    {
        [SerializeField] private SceneReference scene;

        public string GetSceneName() => this.scene.ScenePath;
    }

    [Serializable]
    internal struct PoolSnapshotItem
    {
        [SerializeField, Min(1f)]
        internal int count;
        [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefabs", Filter = "t:GameObject", IsUniqueList = false), Required]
        internal GameObject prefab;
    }
}