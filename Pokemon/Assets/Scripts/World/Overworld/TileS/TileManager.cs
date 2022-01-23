#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Settings.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.TileS
{
    public class TileManager : Manager
    {
        #region Values

        public static TileManager instance;

        [FoldoutGroup("Tile")] [SerializeField]
        private float waitTime = 5;

        private TileSubManager currentTile;
        private readonly List<TileSubManager> allSubManagers = new List<TileSubManager>();

        // ReSharper disable once IdentifierTypo
        private Coroutine currentCorutine;

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
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            yield break;
        }

        public void AddSubManager(TileSubManager add)
        {
            if (add == null || allSubManagers.Contains(add))
                return;

            allSubManagers.Add(add);

            if (allSubManagers.Count != 1) return;

            currentTile = add;
            UpdateNavmesh(add.GetTileName());
        }

        public void RemoveSubManager(TileSubManager remove)
        {
            if (remove == null || !allSubManagers.Contains(remove))
                return;

            allSubManagers.Remove(remove);
        }

        public void UpdateNavmesh(string newSubManager)
        {
            if (currentCorutine != null)
            {
                StopCoroutine(currentCorutine);
                currentCorutine = null;
            }

            currentCorutine = StartCoroutine(BeginUpdate(newSubManager));
        }

        #endregion

        #region Out

        public TileSubManager GetSubManagerByName(string tileName)
        {
            return allSubManagers.FirstOrDefault(tileSubManager => tileSubManager.GetTileName().Equals(tileName));
        }

        #endregion

        #region Internal

        private IEnumerator BeginUpdate(string newSubManager)
        {
            yield return new WaitForSeconds(waitTime);

            if (currentTile != null)
                currentTile.DisableDividers();
            currentTile = GetByName(newSubManager);
            currentTile.EnableDividers();

            yield return NavMeshManager.instance.RebakeWait(currentTile.GetSurface());
        }

        private TileSubManager GetByName(string tileName)
        {
            return allSubManagers.FirstOrDefault(tileSubManager => tileSubManager.GetTileName().Equals(tileName));
        }

        #endregion
    }
}