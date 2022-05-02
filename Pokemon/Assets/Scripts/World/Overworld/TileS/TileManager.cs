#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Common;
using Mfknudsen.Settings.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.Tiles
{
    public class TileManager : Manager
    {
        #region Values

        public static TileManager instance;

        [FoldoutGroup("Tile")] [SerializeField]
        private float waitTime = 5;

        private TileSubManager currentTile;
        private readonly List<TileSubManager> allSubManagers = new List<TileSubManager>();

        private Timer resetWorldPositionTimer;

        private string toLoadName;

        #endregion

        #region Getters

        public TileSubManager GetSubManager()
        {
            return currentTile;
        }

        public TileSubManager[] GetSubManagers()
        {
            return allSubManagers.ToArray();
        }

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);

            yield break;
        }

        public void AddSubManager(TileSubManager add)
        {
            if (add == null || allSubManagers.Contains(add))
                return;

            allSubManagers.Add(add);

            if (allSubManagers.Count != 1) return;

            currentTile = add;
            UpdateNavmesh();
        }

        public void RemoveSubManager(TileSubManager remove)
        {
            if (remove == null || !allSubManagers.Contains(remove))
                return;

            allSubManagers.Remove(remove);
        }

        public void UpdateNavmesh()
        {
            StartCoroutine(BeginUpdate());
        }

        public void HideTiles()
        {
            foreach (TileSubManager tileSubManager in allSubManagers)
                tileSubManager.gameObject.SetActive(false);
        }

        public void ShowTiles()
        {
            foreach (TileSubManager tileSubManager in allSubManagers)
                tileSubManager.gameObject.SetActive(true);
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

                List<Neighbor> toUnload = new List<Neighbor>(),
                    loaded = new List<Neighbor>(),
                    toLoad = new List<Neighbor>();
                toUnload.AddRange(currentTile.GetNeighbors());
                loaded.AddRange(currentTile.GetNeighbors());

                currentTile = allSubManagers.First(m => m.GetTileName().Equals(newSubManagerName));
                currentTile.EnableDividers();

                foreach (Neighbor neighbor in currentTile.GetNeighbors().Where(n => toUnload.Contains(n)))
                    toUnload.Remove(neighbor);

                foreach (Neighbor neighbor in toUnload.Where(n => loaded.Contains(n)))
                    loaded.Remove(neighbor);

                foreach (Neighbor neighbor in currentTile.GetNeighbors().Where(n => loaded.Contains(n)))
                    toLoad.Add(neighbor );

                #region Unload Unneeded Neighbors

                foreach (Neighbor neighbor in toUnload)
                {
                    TileSubManager unload = allSubManagers.First(m => m.GetTileName().Equals(neighbor.GetSceneName()));
                    unload.Unload();
                }

                #endregion

                #region Load New Neighbors

                foreach (Neighbor neighbor in toLoad)
                {
                    WorldManager.instance.LoadSceneAsync(neighbor.GetSceneName());
                }

                #endregion

                ResetWorldCenter();
                UpdateNavmesh();
            });
        }

        #endregion

        #region Out

        public TileSubManager GetSubManagerByName(string tileName)
        {
            return allSubManagers.FirstOrDefault(tileSubManager => tileSubManager.GetTileName().Equals(tileName));
        }

        #endregion

        #region Internal

        private IEnumerator BeginUpdate()
        {
            yield return NavMeshManager.instance.RebakeWait(currentTile.GetSurface());
        }

        private void ResetWorldCenter()
        {
        }

        #endregion
    }
}