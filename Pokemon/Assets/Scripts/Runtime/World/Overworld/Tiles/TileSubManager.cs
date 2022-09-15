#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Common;
using Runtime.Systems;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Tiles
{
    public class TileSubManager : Manager
    {
        #region Values

        [SerializeField] private TileManager tileManager;

        [FoldoutGroup("Tile")] [SerializeField]
        private string tileName;

        [FoldoutGroup("Tile")] [SerializeField]
        private TileBorder[] borders;

        [FoldoutGroup("Tile/Neighbors")] [SerializeField]
        private Neighbor[] neighbors;

        [FoldoutGroup("Tile/Neighbors")] [SerializeField]
        private FakeNeighbor[] fakeNeighbors;

        #endregion

        #region Build In States

        private void OnEnable()
        {
            this.tileManager.AddSubManager(this);
            this.holder.transform.parent = this.tileManager.GetHolderObject().transform;

            GetSurfaces().BuildNavMesh();
        }

        #endregion

        #region Getters

        public string GetTileName() => tileName;

        public NavMeshSurface GetSurfaces() => this.holder.gameObject.GetFirstComponentByRoot<NavMeshSurface>();

        public IEnumerable<Neighbor> GetNeighbors() => neighbors;

        #endregion

        #region In

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
            this.holder.StartCoroutine(UnloadSubManager());
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