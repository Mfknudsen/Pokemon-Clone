#region Packages

using Mfknudsen.Common.CommonPath;
using UnityEngine;

#endregion

namespace Mfknudsen.Editor.MeshGeneration
{
    public class RoadMeshGenerator : MonoBehaviour
    {
        #region Values

        [Range(.05f, 1.5f)] [SerializeField] private float spacing = 1;
        [SerializeField] private float roadWidth = 1;

        #endregion

        #region In

        public void UpdateRoad()
        {
            Path path = GetComponent<PathCreator>().path;
            Vector3[] points = path.CalculateEvenlySpacedPoints(spacing);
            GetComponent<MeshFilter>().mesh = CreateRoadMesh(points);
        }

        #endregion

        #region Internal

        private Mesh CreateRoadMesh(Vector3[] points)
        {
            Vector3[] verts = new Vector3[points.Length * 2];
            int[] tris = new int[2 * (points.Length - 1) * 3];
            int vertIndex = 0, triIndex = 0;

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 current = points[i];
                Vector3 forward = Vector3.zero;

                if (i < points.Length - 1)
                    forward += points[i + 1] - current;
                if (i > 0)
                    forward += current - points[i - 1];

                forward.Normalize();

                Vector3 left = new(-forward.y, forward.x);

                verts[vertIndex] = current + left * roadWidth * 0.5f;
                verts[vertIndex + 1] = current - left * roadWidth * 0.5f;

                if (i < points.Length - 1)
                {
                    tris[triIndex] = vertIndex;
                    tris[triIndex + 1] = vertIndex + 2;
                    tris[triIndex + 2] = vertIndex + 1;

                    tris[triIndex + 3] = vertIndex + 1;
                    tris[triIndex + 4] = vertIndex + 2;
                    tris[triIndex + 5] = vertIndex + 3;
                }

                vertIndex += 2;
                triIndex += 6;
            }

            Mesh mesh = new();
            mesh.vertices = verts;
            mesh.triangles = tris;

            return mesh;
        }

        #endregion
    }
}