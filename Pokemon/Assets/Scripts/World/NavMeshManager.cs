#region Packages

using System.Collections;
using Mfknudsen.Settings.Managers;
using Unity.AI.Navigation;

#endregion

namespace Mfknudsen.World
{
    public class NavMeshManager : Manager
    {
        public static NavMeshManager instance;

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