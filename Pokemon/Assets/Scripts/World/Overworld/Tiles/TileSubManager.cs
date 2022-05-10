#region Packages

using System;
using System.Collections;
using System.Linq;
using Mfknudsen.Settings.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.AI.Navigation;

#endregion

namespace Mfknudsen.World.Overworld.Tiles
{
    public class TileSubManager : Manager
    {
        #region Values

        [FoldoutGroup("Tile")] [SerializeField]
        private string tileName;

        [FoldoutGroup("Tile")] [SerializeField]
        private NavMeshSurface surface;

        [FoldoutGroup("Tile")] [SerializeField]
        private TileBorder[] borders;

        [FoldoutGroup("Tile/Neighbors")] [SerializeField]
        private Neighbor[] neighbors;

        [FoldoutGroup("Tile/Neighbors")] [SerializeField]
        private FakeNeighbor[] fakeNeighbors;

        #endregion

        #region Getters

        public string GetTileName()
        {
            return tileName;
        }

        public NavMeshSurface GetSurface()
        {
            return surface;
        }

        public Neighbor[] GetNeighbors()
        {
            return neighbors;
        }

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            TileManager tileManager = TileManager.instance;
            tileManager.AddSubManager(this);
            transform.parent = tileManager.transform;

            yield break;
        }

        public void EnableDividers()
        {
            foreach (GameObject divider in borders.Select(b => b.gameObject))
                divider.SetActive(true);
        }

        public void DisableDividers()
        {
            foreach (GameObject divider in borders.Select(b => b.gameObject))
                divider.SetActive(false);
        }

        public void Unload()
        {
            StartCoroutine(UnloadSubManager());
        }
        
        #endregion

        #region Internal

        private IEnumerator UnloadSubManager()
        {
            yield break;
        }

        #endregion
    }

    [Serializable]
    public struct Neighbor
    {
        #region Values

        [SerializeField] private string sceneName;
        [SerializeField] private Transform loadAnchor;

        #endregion

        #region Getters

        public string GetSceneName()
        {
            return sceneName;
        }

        #endregion
    }

    [Serializable]
    public struct FakeNeighbor
    {
        [SerializeField] private string sceneName;
        [SerializeField] private Transform loadAnchor;
    }
}