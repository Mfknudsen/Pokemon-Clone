#region Packages

using System;
using Mfknudsen.Settings.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.World.Overworld.TileS
{
    public class TileSubManager : Manager
    {
        #region Values

        [FoldoutGroup("Tile")] [SerializeField]
        private string tileName;

        [FoldoutGroup("Tile")] [SerializeField]
        private NavMeshSurface surface;

        [FoldoutGroup("Tile")] [SerializeField]
        private GameObject[] dividers;

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

        #endregion

        #region In

        public override void Setup()
        {
            TileManager tileManager = TileManager.instance;
            tileManager.AddSubManager(this);
            transform.parent = tileManager.transform;
        }

        public void EnableDividers()
        {
            foreach (GameObject divider in dividers)
                divider.SetActive(true);
        }

        public void DisableDividers()
        {
            foreach (GameObject divider in dividers)
                divider.SetActive(false);
        }

        public void LoadNeighbors()
        {
        }

        public void UnloadNeighbors()
        {
        }

        #endregion
    }

    [Serializable]
    internal struct Neighbor
    {
        [SerializeField] private string sceneName;
        [SerializeField] private Transform loadAnchor;
    }

    [Serializable]
    internal struct FakeNeighbor
    {
        [SerializeField] private string sceneName;
        [SerializeField] private Transform loadAnchor;
    }
}