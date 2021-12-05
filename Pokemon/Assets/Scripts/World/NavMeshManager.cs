#region Packages

using System.Collections;
using Mfknudsen.Settings.Manager;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.World
{
    public class NavMeshManager : Manager
    {
        public static NavMeshManager instance;

        public override void Setup()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Rebake(NavMeshSurface surface)
        {
            if (surface.navMeshData != null)
                surface.UpdateNavMesh(surface.navMeshData);
            else surface.BuildNavMesh();
        }

        public IEnumerator RebakeWait(NavMeshSurface surface)
        {
            if (surface.navMeshData != null)
                surface.UpdateNavMesh(surface.navMeshData);
            else surface.BuildNavMesh();

            yield break;
        }
    }
}