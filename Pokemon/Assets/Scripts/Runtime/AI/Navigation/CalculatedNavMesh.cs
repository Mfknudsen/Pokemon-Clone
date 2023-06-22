#region Libraries

using Runtime.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    public sealed class CalculatedNavMesh : ScriptableObject
    {
        #region Values

        [SerializeField] private Vector2[] vertices2D;
        [SerializeField] private float[] verticesY;

        [SerializeField] private NavTriangle[] triangles;
        [SerializeField] private int[] areaType;
        [SerializeField] private Dictionary<int, List<NavigationPointEntry>> navigationEntryPoints;

        private NavigationPoint[] navigationPoints;
        private NavigationPointEntry[] entryPoints;

        #endregion

        #region Setters

        public void SetValues(Vector3[] vertices, NavTriangle[] triangles, int[] areaType, Dictionary<int, List<NavigationPointEntry>> navigationEntryPoints)
        {
            this.triangles = triangles;
            this.areaType = areaType;
            this.navigationEntryPoints = navigationEntryPoints;

            this.vertices2D = new Vector2[vertices.Length];
            this.verticesY = new float[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float x = vertices[i].x, y = vertices[i].y, z = vertices[i].z;
                this.vertices2D[i] = new(x, z);
                this.verticesY[i] = y;
            }
        }

        public void SetNavigationPoints(NavigationPoint[] set)
        {
            this.navigationPoints = set;
            this.entryPoints = this.navigationPoints.SelectMany(p => p.GetEntryPoints()).ToArray();
        }

        public void SetVert(int index, Vector3 v)
        {
            this.vertices2D[index] = new Vector2(v.x, v.z); ;
            this.verticesY[index] = v.y;
        }

        #endregion

        #region Getters

        public Vector2[] SimpleVertices => this.vertices2D;

        public NavTriangle[] Triangles => this.triangles;

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

        public int[] GetInds => this.triangles.SelectMany(t => t.Vertices).ToArray();

        #endregion
    }

    [Serializable]
    public sealed class NavTriangle
    {
        #region Values

        [SerializeField]
        private int A, B, C;

        [SerializeField]
        private int area;

        [SerializeField]
        private int[] neighborIDs;
        [SerializeField]
        private float[] widthDistanceBetweenNeighbor;

        [SerializeField]
        private int[] navPointIDs;

        #endregion

        #region Build In States

        public NavTriangle(int A, int B, int C, int area)
        {
            this.A = A;
            this.B = B;
            this.C = C;

            this.area = area;
        }

        #endregion

        #region Getters

        public int[] Vertices => new int[] { this.A, this.B, this.C };

        public int[] Neighbors => this.neighborIDs;

        public int[] NavPoints => this.navPointIDs;

        public int Area => this.area;

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetNeighborIDs(int[] ids) => this.neighborIDs = ids;

        public void SetNavPointIDs(int[] set) => this.navPointIDs = set;
#endif

        #endregion

        #region In

#if UNITY_EDITOR

        public void SetBorderWidth(Vector3[] verts, NavTriangle[] triangles)
        {
            if (this.neighborIDs.Length == 0)
                return;

            this.widthDistanceBetweenNeighbor = new float[this.neighborIDs.Length];

            for (int i = 0; i < this.neighborIDs.Length; i++)
            {
                int otherID = this.neighborIDs[i];
                NavTriangle other = triangles[otherID];
                int[] ids = other.Vertices.SharedBetween(this.Vertices);

                if (ids.Length != 2)
                    continue;

                float dist = Vector3.Distance(verts[ids[0]], verts[ids[1]]);

                if (i + 2 < this.neighborIDs.Length)
                {
                    int connectedBorderNeighbor = -1;
                    if (other.neighborIDs.Contains(this.neighborIDs[i + 2]))
                        connectedBorderNeighbor = i + 2;
                    else if (other.neighborIDs.Contains(this.neighborIDs[i + 1]))
                        connectedBorderNeighbor = i + 1;

                    if (connectedBorderNeighbor > -1)
                    {
                        ids = triangles[this.neighborIDs[connectedBorderNeighbor]].Vertices.SharedBetween(this.Vertices);
                        if (ids.Length == 2)
                        {
                            dist += Vector3.Distance(verts[ids[0]], verts[ids[1]]);
                            this.widthDistanceBetweenNeighbor[connectedBorderNeighbor] = dist;
                        }
                    }
                }
                else if (i + 1 < this.neighborIDs.Length)
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

        public Vector2 Center(Vector2[] verts)
            => Vector2.Lerp(Vector2.Lerp(verts[this.A], verts[this.B], .5f), verts[this.C], .5f);

#if UNITY_EDITOR
        public Vector3 Center(Vector3[] vertices) =>
            Vector3.Lerp(Vector3.Lerp(vertices[this.A], vertices[this.B], .5f), vertices[this.C], .5f);
#endif

        #endregion
    }
}