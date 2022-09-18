#region Packages

using System.Collections;
using Runtime.Systems;
using Unity.AI.Navigation;
using UnityEngine;

#endregion

namespace Runtime.World
{
    [CreateAssetMenu(menuName = "Manager/Navigation")]
    public class NavMeshManager : Manager
    {
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