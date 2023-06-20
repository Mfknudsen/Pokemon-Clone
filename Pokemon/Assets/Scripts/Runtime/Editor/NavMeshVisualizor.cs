#region Libraries

using Assets.Scripts.Runtime.World.Overworld;
using Runtime.AI.Navigation;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace Runtime.Editor
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public sealed class NavMeshVisualizor : MonoBehaviour
    {
        #region Build In States

#if UNITY_EDITOR
        public void Create()
        {
            Mesh mesh = new() { name = "Navmesh" };

            CalculatedNavMesh calculatedNavmesh = this.transform.root.GetComponent<TileSubController>().GetNavmesh;

            mesh.SetVertices(calculatedNavmesh.Vertices());
            mesh.SetIndices(calculatedNavmesh.Triangles.SelectMany(t => t.Vertices).ToArray(), MeshTopology.Triangles, 0);
            mesh.SetNormals(calculatedNavmesh.Vertices().Select(v => Vector3.up).ToArray());
            mesh.RecalculateNormals();

            this.GetComponent<MeshRenderer>().material =
                AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Editor/NavmeshVisualizor.mat");
            this.GetComponent<MeshFilter>().mesh = mesh;
        }
#endif

        #endregion
    }
}