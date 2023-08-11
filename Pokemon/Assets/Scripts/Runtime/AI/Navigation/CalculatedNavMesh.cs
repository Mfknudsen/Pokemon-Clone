#region Libraries

using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Core;
using Runtime.Variables;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    public sealed class CalculatedNavMesh : SerializedScriptableObject
    {
        #region Values

        [SerializeField] private Vector2[] vertices2D;
        [SerializeField] private float[] verticesY;

        [SerializeField] private NavTriangle[] triangles;
        [SerializeField] private int[] areaType;
        [SerializeField] private SDictionary<int, List<NavigationPointEntry>> navigationEntryPoints;

        [SerializeField] private SDictionary<Vector2Int, List<int>> trianglesByVertexPosition;

        /// <summary>
        /// Index of vertex returns all NavTriangles containing the vertex id.
        /// </summary>
        [SerializeField] private List<List<int>> triangleByVertexID;

        private NavigationPoint[] navigationPoints;

        private readonly float groupDivision = 10f;
        [SerializeField, HideInInspector] private int minFloorX, minFloorY, maxFloorX, maxFloorY;

        #endregion

        #region Getters

        public Vector2[] SimpleVertices => this.vertices2D;

        public NavTriangle[] Triangles => this.triangles;

        public int[] Areas => this.areaType;

        public NavTriangle[] GetTrianglesByVertexID(int id) =>
            this.triangleByVertexID[id].Select(i => this.triangles[i]).ToArray();

        #endregion

        #region Setters

        public void SetValues(Vector3[] vertices, NavTriangle[] navTriangles, int[] areaTypes,
            Dictionary<int, List<NavigationPointEntry>> entryPoints)
        {
            this.triangles = new NavTriangle[navTriangles.Length];
            for (int i = 0; i < navTriangles.Length; i++)
                this.triangles[i] = navTriangles[i];

            this.areaType = areaTypes;
            this.navigationEntryPoints = entryPoints as SDictionary<int, List<NavigationPointEntry>>;

            this.vertices2D = new Vector2[vertices.Length];
            this.verticesY = new float[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float x = vertices[i].x, y = vertices[i].y, z = vertices[i].z;
                this.vertices2D[i] = new Vector2(x, z);
                this.verticesY[i] = y;
            }

            this.trianglesByVertexPosition = new SDictionary<Vector2Int, List<int>>();

            this.triangleByVertexID = new List<List<int>>();
            for (int i = 0; i < vertices.Length; i++)
                this.triangleByVertexID.Add(new List<int>());

            navTriangles.ForEach(t =>
            {
                int a = t.Vertices[0], b = t.Vertices[1], c = t.Vertices[2];
                Vector2Int A = new(Mathf.FloorToInt(this.vertices2D[a].x / this.groupDivision),
                        Mathf.FloorToInt(this.vertices2D[a].y / this.groupDivision)),
                    B = new(Mathf.FloorToInt(this.vertices2D[b].x / this.groupDivision),
                        Mathf.FloorToInt(this.vertices2D[b].y / this.groupDivision)),
                    C = new(Mathf.FloorToInt(this.vertices2D[c].x / this.groupDivision),
                        Mathf.FloorToInt(this.vertices2D[c].y / this.groupDivision));

                this.minFloorX = Mathf.Min(this.minFloorX, Mathf.Min(Mathf.Min(A.x, B.x), C.x));
                this.minFloorY = Mathf.Min(this.minFloorY, Mathf.Min(Mathf.Min(A.y, B.y), C.y));

                this.maxFloorX = Mathf.Max(this.minFloorX, Mathf.Max(Mathf.Max(A.x, B.x), C.x));
                this.maxFloorY = Mathf.Max(this.minFloorY, Mathf.Max(Mathf.Max(A.y, B.y), C.y));

                if (this.trianglesByVertexPosition.TryGetValue(A, out List<int> list))
                    list.Add(t.ID);
                else
                    this.trianglesByVertexPosition.Add(A, new List<int> { t.ID });

                if (this.trianglesByVertexPosition.TryGetValue(B, out list))
                    list.Add(t.ID);
                else
                    this.trianglesByVertexPosition.Add(B, new List<int> { t.ID });

                if (this.trianglesByVertexPosition.TryGetValue(C, out list))
                    list.Add(t.ID);
                else
                    this.trianglesByVertexPosition.Add(C, new List<int> { t.ID });

                this.triangleByVertexID[a].Add(t.ID);
                this.triangleByVertexID[b].Add(t.ID);
                this.triangleByVertexID[c].Add(t.ID);
            });
        }

        public void SetVert(int index, Vector3 v)
        {
            this.vertices2D[index] = new Vector2(v.x, v.z);
            ;
            this.verticesY[index] = v.y;
        }

        #endregion

        #region Out

        public Vector3[] Vertices()
        {
            Vector3[] result = new Vector3[this.verticesY.Length];
            for (int i = 0; i < this.verticesY.Length; i++)
                result[i] = new Vector3(this.vertices2D[i].x, this.verticesY[i], this.vertices2D[i].y);
            return result;
        }

        public NavTriangle[] GetByIndex(int index) => this.triangles.Where(t => t.Vertices.Contains(index)).ToArray();

        public int[] Inds => this.triangles.SelectMany(t => t.Vertices).ToArray();

        public int ClosestTriangleIndex(Vector3 p)
        {
            Vector2 p2D = p.XZ();

            for (int i = 0; i < this.triangles.Length; i++)
            {
                int[] ids = this.triangles[i].Vertices;
                if (MathC.PointWithinTriangle2D(p2D,
                        this.SimpleVertices[ids[0]],
                        this.SimpleVertices[ids[1]],
                        this.SimpleVertices[ids[2]]))
                    return this.triangles[i].ID;
            }

            return this.triangles.RandomFrom().ID;
        }

        public int ClosestTriangleIndex(Vector2 p)
        {
            for (int i = 0; i < this.triangles.Length; i++)
            {
                int[] ids = this.triangles[i].Vertices;
                if (MathC.PointWithinTriangle2D(p,
                        this.SimpleVertices[ids[0]],
                        this.SimpleVertices[ids[1]],
                        this.SimpleVertices[ids[2]]))
                    return this.triangles[i].ID;
            }

            return this.triangles.RandomFrom().ID;
        }

        public Vector3 VertByIndex(int i) =>
            new(this.vertices2D[i].x, this.verticesY[i], this.vertices2D[i].y);

        #endregion
    }

    [Serializable]
    public struct NavTriangle
    {
        #region Values

        [SerializeField] private int id;

        [SerializeField] private int A, B, C;

        [SerializeField] private int area;

        [SerializeField] private float maxY;

        [SerializeField] private List<int> neighborIDs;
        [SerializeField] private List<float> widthDistanceBetweenNeighbor;
        [SerializeField] private List<int> navPointIDs;

        #endregion

        #region Build In States

        public NavTriangle(int id, int A, int B, int C, int area, params Vector3[] verts)
        {
            this.id = id;
            this.A = A;
            this.B = B;
            this.C = C;

            this.maxY = Mathf.Max(Mathf.Max(verts[0].y, verts[1].y), verts[2].y);

            this.area = area;

            this.neighborIDs = new List<int>();
            this.navPointIDs = new List<int>();
            this.widthDistanceBetweenNeighbor = new List<float>();
        }

        #endregion

        #region Getters

        public readonly int GetA => this.A;

        public readonly int GetB => this.B;

        public readonly int GetC => this.C;

        public readonly int[] Vertices => new int[] { this.A, this.B, this.C };

        public readonly List<int> Neighbors => this.neighborIDs;

        public readonly List<int> NavPoints => this.navPointIDs;

        public readonly List<float> Widths => this.widthDistanceBetweenNeighbor;

        public readonly int Area => this.area;

        public readonly int ID => this.id;

        public readonly float MaxY => this.maxY;

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetNeighborIDs(int[] set)
        {
            this.neighborIDs.Clear();
            for (int i = 0; i < set.Length; i++)
            {
                if (!this.neighborIDs.Contains(set[i]))
                    this.neighborIDs.Add(set[i]);
            }
        }

        public void SetNavPointIDs(int[] set)
        {
            this.navPointIDs.Clear();
            for (int i = 0; i < set.Length; i++)
            {
                if (!this.navPointIDs.Contains(set[i]))
                    this.navPointIDs.Add(set[i]);
            }
        }
#endif

        #endregion

        #region In

#if UNITY_EDITOR

        public void SetBorderWidth(List<Vector3> verts, List<NavTriangle> triangles)
        {
            if (this.neighborIDs.Count == 0)
                return;

            for (int i = 0; i < this.neighborIDs.Count; i++)
                this.widthDistanceBetweenNeighbor.Add(0f);

            for (int i = 0; i < this.neighborIDs.Count; i++)
            {
                int otherID = this.neighborIDs[i];
                NavTriangle other = triangles[otherID];
                int[] ids = other.Vertices.SharedBetween(this.Vertices);

                if (ids.Length != 2)
                    continue;

                float dist = Vector3.Distance(verts[ids[0]], verts[ids[1]]);

                if (i + 2 < this.neighborIDs.Count)
                {
                    int connectedBorderNeighbor = -1;
                    if (other.neighborIDs.Contains(this.neighborIDs[i + 2]))
                        connectedBorderNeighbor = i + 2;
                    else if (other.neighborIDs.Contains(this.neighborIDs[i + 1]))
                        connectedBorderNeighbor = i + 1;

                    if (connectedBorderNeighbor > -1)
                    {
                        ids = triangles[this.neighborIDs[connectedBorderNeighbor]].Vertices
                            .SharedBetween(this.Vertices);
                        if (ids.Length == 2)
                        {
                            dist += Vector3.Distance(verts[ids[0]], verts[ids[1]]);
                            this.widthDistanceBetweenNeighbor[connectedBorderNeighbor] = dist;
                        }
                    }
                }
                else if (i + 1 < this.neighborIDs.Count)
                {
                    if (other.neighborIDs.Contains(this.neighborIDs[i + 1]))
                    {
                        ids = triangles[this.neighborIDs[i + 1]].Vertices.SharedBetween(this.Vertices);
                        if (ids.Length == 2)
                        {
                            dist += Vector3.Distance(verts[ids[0]], verts[ids[1]]);
                            this.widthDistanceBetweenNeighbor[i + 1] = dist;
                        }
                    }
                }

                this.widthDistanceBetweenNeighbor[i] = dist;
            }
        }

#endif

        #endregion

        #region Out

        public Vector3 Center(Vector3[] verts) =>
            Vector3.Lerp(Vector3.Lerp(verts[this.A], verts[this.B], .5f), verts[this.C], .5f);

        #endregion
    }
}