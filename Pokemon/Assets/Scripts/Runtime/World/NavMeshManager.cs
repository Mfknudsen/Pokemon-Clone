#region Packages

using System.Collections;
using Runtime.Systems;
using Unity.AI.Navigation;

#endregion

namespace Runtime.World
{
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