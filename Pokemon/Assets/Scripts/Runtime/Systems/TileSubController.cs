#region Packages

using System;
using System.Collections.Generic;
using Runtime.World.Overworld.Tiles;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public class TileSubController : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private TileManager tileManager;

        [SerializeField] private Neighbor[] neighbors;
        
        [SerializeField, Required] private NavMeshSurface navMeshSurface;

        #endregion

        #region Build In States

        private void OnEnable() => this.tileManager.AddSubManager(this);

        private void OnDisable() => this.tileManager.RemoveSubManager(this);

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
}