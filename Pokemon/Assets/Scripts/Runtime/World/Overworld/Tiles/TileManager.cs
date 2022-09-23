#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Common;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Tiles
{
    [CreateAssetMenu(menuName = "Manager/Tile")]
    public class TileManager : Manager
    {
        #region Values

        [SerializeField, Required] private NavMeshManager navMeshManager;
        [SerializeField, Required] private WorldManager worldManager;

        [FoldoutGroup("Tile")] [SerializeField]
        private float waitTime = 5;

        private TileSubController currentTile;
        private readonly List<TileSubController> allSubManagers = new();

        private Timer resetWorldPositionTimer;

        private string toLoadName;

        #endregion

        #region Getters

        public TileSubController GetSubManager()
        {
            return this.currentTile;
        }

        public TileSubController[] GetSubManagers()
        {
            return this.allSubManagers.ToArray();
        }

        #endregion

        #region In

        public void AddSubManager(TileSubController add)
        {
            if (add == null || this.allSubManagers.Contains(add))
                return;

            this.allSubManagers.Add(add);

            if (this.allSubManagers.Count != 1) return;

            this.currentTile = add;
            UpdateNavmesh();
        }

        public void RemoveSubManager(TileSubController remove)
        {
            if (remove == null || !this.allSubManagers.Contains(remove))
                return;

            this.allSubManagers.Remove(remove);
        }

        private void UpdateNavmesh()
        {
            this.currentTile.StartCoroutine(BeginUpdate());
        }

        public void HideTiles()
        {
            foreach (TileSubController tileSubController in this.allSubManagers)
                tileSubController.gameObject.SetActive(false);
        }

        public void ShowTiles()
        {
            foreach (TileSubController tileSubController in this.allSubManagers)
                tileSubController.gameObject.SetActive(true);
        }

        public void SetCurrentSubTile(string newSubManagerName)
        {
            if (this.toLoadName.Equals(newSubManagerName)) return;

            this.toLoadName = newSubManagerName;

            this.resetWorldPositionTimer?.Stop();

            this.resetWorldPositionTimer = new Timer(this.waitTime);
            this.resetWorldPositionTimer.timerEvent.AddListener(() =>
            {
                this.currentTile.DisableDividers();

                List<Neighbor> toUnload = new(),
                    loaded = new(),
                    toLoad = new();
                toUnload.AddRange(this.currentTile.GetNeighbors());
                loaded.AddRange(this.currentTile.GetNeighbors());

                this.currentTile = this.allSubManagers.First(m => m.GetTileName().Equals(newSubManagerName));
                this.currentTile.EnableDividers();

                foreach (Neighbor neighbor in this.currentTile.GetNeighbors().Where(n => toUnload.Contains(n)))
                    toUnload.Remove(neighbor);

                foreach (Neighbor neighbor in toUnload.Where(n => loaded.Contains(n)))
                    loaded.Remove(neighbor);

                toLoad.AddRange(this.currentTile.GetNeighbors().Where(n => loaded.Contains(n)));

                #region Unload Unneeded Neighbors

                foreach (TileSubController unload in toUnload.Select(neighbor => this.allSubManagers.First(m => m.GetTileName().Equals(neighbor.GetSceneName()))))
                    unload.Unload();

                #endregion

                #region Load New Neighbors

                foreach (Neighbor neighbor in toLoad)
                {
                    this.worldManager.LoadSceneAsync(neighbor.GetSceneName());
                }

                #endregion

                ResetWorldCenter();
                UpdateNavmesh();
            });
        }

        #endregion

        #region Out

        public TileSubController GetSubManagerByName(string tileName) => this.allSubManagers.FirstOrDefault(tileSubManager => tileSubManager.GetTileName().Equals(tileName));

        #endregion

        #region Internal

        private IEnumerator BeginUpdate()
        {
            yield return this.navMeshManager.RebakeWait(this.currentTile.GetSurface ());
        }

        private void ResetWorldCenter()
        {
        }

        #endregion
    }
}