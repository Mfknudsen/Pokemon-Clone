#region Libraries

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation;
using Runtime.Algorithms.BattleSetup;
using Runtime.Editor;
using Runtime.Systems.Pooling;
using Runtime.Variables;
using Runtime.World.Overworld.TileHierarchy;
using Runtime.World.Overworld.Tiles;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Runtime.World.Overworld
{
    public sealed class TileSubController : MonoBehaviour
    {
        #region Values

        [SerializeField] [Required] private TileManager tileManager;

        [SerializeField] private Neighbor[] neighbors;

        [SerializeField] [FoldoutGroup("Navigation")] [Required]
        private NavMeshSurface navMeshSurface;

        [SerializeField] [FoldoutGroup("Navigation")]
        private Vector3 navmeshCleanUpPoint;

        [SerializeField] private PoolSnapshotItem[] poolingSnapshot;

        [FormerlySerializedAs("calculatedNavMesh")] [SerializeField] [FoldoutGroup("Navigation")]
        private NavigationMesh navigationMesh;

        [SerializeField] [FoldoutGroup("Hierarchy", 1)] [Required]
        private TileEnvironment tileEnvironment;

        [SerializeField] [FoldoutGroup("Hierarchy")] [Required]
        private TileSpawners tileSpawners;

        [SerializeField] [FoldoutGroup("Hierarchy")] [Required]
        private TileAI tileAI;

        [SerializeField] [FoldoutGroup("Hierarchy")] [Required]
        private TileConnectionPoints tileConnectionPoints;

        [SerializeField] [FoldoutGroup("Hierarchy")] [Required]
        private TileLighting tileLighting;

        [SerializeField] [FoldoutGroup("Optimize", -1)]
        private TileOptimizedInformation optimizedInformation;

        #endregion

        #region Build In States

        private void OnValidate() =>
            this.name = this.gameObject.scene.name + " - TileSubController";

        private void Start()
        {
            this.SetAsCurrent();
            this.navMeshSurface.enabled = false;
        }

        private void OnEnable()
        {
            UnitNavigation.SetNavMesh(this.navigationMesh);

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

        public TileEnvironment GetTileEnvironment() => this.tileEnvironment;

        public TileSpawners GetTileSpawners() => this.tileSpawners;

        public TileAI GetTileAI() => this.tileAI;

        public TileLighting GetTileLighting() => this.tileLighting;

        public TileConnectionPoints GetTileConnectionPoints() => this.tileConnectionPoints;

#if UNITY_EDITOR
        public NavigationMesh GetNavmesh => this.navigationMesh;

        public Vector3 GetCleanUpPoint => this.navmeshCleanUpPoint;
#endif

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetCalculatedNavMesh(NavigationMesh set) => this.navigationMesh = set;

        public void SetCleanUpPoint(Vector3 set) => this.navmeshCleanUpPoint = set;

        public void SetOptimizedInformation(TileOptimizedInformation set) => this.optimizedInformation = set;
#endif

        #endregion

        #region In

        public void SetAsCurrent()
        {
            SetupBattleWorldLayout.SetOptimizedWorldInformation(this.optimizedInformation);
            UnitNavigation.SetNavMesh(this.navigationMesh);
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
            return FindObjectsByType<NavigationPoint>(FindObjectsSortMode.None)
                .OrderBy(p => p.gameObject.scene.name)
                .ThenBy(p => p.gameObject.GetInstanceID())
                .ToArray();
        }

        #endregion

        #region Internal

#if UNITY_EDITOR
        [FoldoutGroup("Hierarchy")]
        [PropertyOrder(-1)]
        [Button]
        private void CreateHierarchy()
        {
            if (this.transform.Find("Editor") == null)
            {
                GameObject editorObject = new GameObject("Editor");
                editorObject.transform.parent = this.transform;
                GameObject navMeshVisualizer = new GameObject("NavMesh Visualizer");
                navMeshVisualizer.transform.parent = editorObject.transform;
                navMeshVisualizer.AddComponent<NavMeshVisualizer>();
            }

            if (this.tileEnvironment == null)
            {
                this.tileEnvironment = this.gameObject.GetComponentInChildren<TileEnvironment>();
                this.tileEnvironment ??= new GameObject("Environment").AddComponent<TileEnvironment>();
                this.tileEnvironment.transform.parent = this.transform;
            }

            if (this.tileSpawners == null)
            {
                this.tileSpawners = this.gameObject.GetComponentInChildren<TileSpawners>();
                this.tileSpawners ??= new GameObject("Spawners").AddComponent<TileSpawners>();
                this.tileSpawners.transform.parent = this.transform;
            }

            if (this.tileAI == null)
            {
                this.tileAI = this.gameObject.GetComponentInChildren<TileAI>();
                this.tileAI ??= new GameObject("AI").AddComponent<TileAI>();
                this.tileAI.transform.parent = this.transform;
            }

            if (this.tileLighting == null)
            {
                this.tileLighting = this.gameObject.GetComponentInChildren<TileLighting>();
                this.tileLighting ??= new GameObject("Lighting").AddComponent<TileLighting>();
                this.tileLighting.transform.parent = this.transform;
            }

            if (this.tileConnectionPoints != null) return;

            this.tileConnectionPoints = this.gameObject.GetComponentInChildren<TileConnectionPoints>();
            this.tileConnectionPoints ??= new GameObject("Connection Points").AddComponent<TileConnectionPoints>();
            this.tileConnectionPoints.transform.parent = this.transform;
        }
#endif

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
        [SerializeField] [Min(1f)] internal int count;

        [SerializeField]
        [AssetsOnly]
        [AssetSelector(Paths = "Assets/Prefabs", Filter = "t:GameObject", IsUniqueList = false)]
        [Required]
        internal GameObject prefab;
    }
}