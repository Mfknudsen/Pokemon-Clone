#region Libraries

using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation;
using Runtime.Algorithms.PathFinding;
using Runtime.Core;
using UnityEngine;

#endregion

namespace Runtime
{
    public sealed class NavTester : MonoBehaviour
    {
        #region Values

        [SerializeField] private NavTriangle[] triangles;

        [SerializeField] private Transform[] vertices;

        private CalculatedNavMesh temp;

        private List<Portal> portals;

        private List<Vector3> path;

        [SerializeField] private Transform start, end;

        private Dictionary<int, RemappedVert> remapped;

        #endregion

        #region Build In States

        private void Start()
        {
            this.temp = ScriptableObject.CreateInstance<CalculatedNavMesh>();
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> simple = new List<Vector2>();
            foreach (Transform t in this.vertices)
            {
                verts.Add(t.position);
                simple.Add(verts[^1].XZ());
            }

            this.temp.SetValues(verts.ToArray(),
                this.triangles,
                new int[this.triangles.Length],
                new Dictionary<int, List<NavigationPointEntry>>());

            UnitNavigation.SetNavMesh(this.temp);

            List<int> triangleIDs = new List<int>();
            for (int i = 0; i < this.triangles.Length; i++)
                triangleIDs.Add(i);

            this.path = Funnel.GetPath(this.start.position,
                this.end.position,
                triangleIDs.ToArray(),
                this.triangles,
                verts.ToArray(),
                this.start.GetComponent<UnitAgent>(),
                out this.portals,
                out this.remapped);
        }

        private void OnDestroy() =>
            Destroy(this.temp);

        private void OnDrawGizmos()
        {
            foreach (NavTriangle navTriangle in this.triangles)
            {
                Vector3 a = this.vertices[navTriangle.GetA].position,
                    b = this.vertices[navTriangle.GetB].position,
                    c = this.vertices[navTriangle.GetC].position;
                Debug.DrawLine(a, b, Color.red);
                Debug.DrawLine(c, b, Color.red);
                Debug.DrawLine(c, a, Color.red);
            }

            if (this.path is { Count: > 0 })
            {
                Debug.DrawLine(this.start.position, this.path[0], Color.yellow);
                for (int i = 1; i < this.path.Count; i++)
                    Debug.DrawLine(this.path[i], this.path[i - 1], Color.yellow);
            }

            if (this.portals != null && this.remapped != null)
            {
                Vector3[] pos = this.remapped.Values.Select(r => r.vert).ToArray();
                for (int i = 1; i < this.portals.Count; i++)
                {
                    Debug.DrawLine(pos[this.portals[i].left]+ Vector3.up,
                        pos[this.portals[i - 1].left] + Vector3.up, Color.blue);
                    Debug.DrawLine(pos[this.portals[i].right] + Vector3.up,
                        pos[this.portals[i - 1].right] + Vector3.up, Color.cyan);
                }
            }
        }

        #endregion
    }
}