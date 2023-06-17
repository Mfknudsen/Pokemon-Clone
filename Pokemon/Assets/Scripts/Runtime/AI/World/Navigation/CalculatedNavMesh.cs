#region Libraries

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.AI.World.Navigation
{
    public sealed class CalculatedNavMesh : ScriptableObject
    {
        #region Values

        [SerializeField] private Vector2[] vertices2D;
        [SerializeField] private float[] verticesY;

        [SerializeField] private NavTriangle[] triangles;
        [SerializeField] private int[] areaType;
        [SerializeField] private Dictionary<int, List<NavigationPointEntry>> navigationEntryPoints;

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
                this.vertices2D[i] = new(vertices[i].x, vertices[i].z);
                this.verticesY[i] = vertices[i].y;
            }
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
        public void SetNavPointIDs(int[] set) => this.navPointIDs = set;
#endif

        #endregion

        #region In

#if UNITY_EDITOR
        public void SetNeighborIDs(int[] set)
        {
            this.neighborIDs = set;

            this.widthDistanceBetweenNeighbor = new float[this.neighborIDs.Length];

            for (int i = 0; i < this.neighborIDs.Length; i++)
            {

            }
        }
#endif

        #endregion

        #region Out

#if UNITY_EDITOR
        public Vector3 Center(Vector3[] vertices) => Vector3.Lerp(Vector3.Lerp(vertices[this.A], vertices[this.B], .5f), vertices[this.C], .5f);
#endif

        #endregion
    }
}