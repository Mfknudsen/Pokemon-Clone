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
            return currentTile;
        }

        public TileSubController[] GetSubManagers()
        {
            return allSubManagers.ToArray();
        }

        #endregion

        #region In

        public void AddSubManager(TileSubController add)
        {
            if (add == null || allSubManagers.Contains(add))
                return;

            allSubManagers.Add(add);

            if (allSubManagers.Count != 1) return;

            currentTile = add;
            UpdateNavmesh();
        }

        public void RemoveSubManager(TileSubController remove)
        {
            if (remove == null || !allSubManagers.Contains(remove))
                return;

            allSubManagers.Remove(remove);
        }

        private void UpdateNavmesh()
        {
            this.currentTile.StartCoroutine(BeginUpdate());
        }

        public void HideTiles()
        {
            foreach (TileSubController tileSubController in allSubManagers)
                tileSubController.gameObject.SetActive(false);
        }

        public void ShowTiles()
        {
            foreach (TileSubController tileSubController in allSubManagers)
                tileSubController.gameObject.SetActive(true);
        }

        public void SetCurrentSubTile(string newSubManagerName)
        {
            if (toLoadName.Equals(newSubManagerName)) return;

            toLoadName = newSubManagerName;

            resetWorldPositionTimer?.Stop();

            resetWorldPositionTimer = new Timer(waitTime);
            resetWorldPositionTimer.timerEvent.AddListener(() =>
            {
                currentTile.DisableDividers();

                List<Neighbor> toUnload = new(),
                    loaded = new(),
                    toLoad = new();
                toUnload.AddRange(currentTile.GetNeighbors());
                loaded.AddRange(currentTile.GetNeighbors());

                currentTile = allSubManagers.First(m => m.GetTileName().Equals(newSubManagerName));
                currentTile.EnableDividers();

                foreach (Neighbor neighbor in currentTile.GetNeighbors().Where(n => toUnload.Contains(n)))
                    toUnload.Remove(neighbor);

                foreach (Neighbor neighbor in toUnload.Where(n => loaded.Contains(n)))
                    loaded.Remove(neighbor);

                toLoad.AddRange(currentTile.GetNeighbors().Where(n => loaded.Contains(n)));

                #region Unload Unneeded Neighbors

                foreach (TileSubController unload in toUnload.Select(neighbor =>
                             allSubManagers.First(m => m.GetTileName().Equals(neighbor.GetSceneName()))))
                    unload.Unload();

                #endregion

                #region Load New Neighbors

                foreach (Neighbor neighbor in toLoad)
                {
                    worldManager.LoadSceneAsync(neighbor.GetSceneName());
                }

                #endregion

                ResetWorldCenter();
                UpdateNavmesh();
            });
        }

        #endregion

        #region Out

        public TileSubController GetSubManagerByName(string tileName) => allSubManagers.FirstOrDefault(tileSubManager => tileSubManager.GetTileName().Equals(tileName));

        #endregion

        #region Internal

        private IEnumerator BeginUpdate()
        {
            yield return this.navMeshManager.RebakeWait(currentTile.GetSurface ());
        }

        private void ResetWorldCenter()
        {
        }

        #endregion
    }
}