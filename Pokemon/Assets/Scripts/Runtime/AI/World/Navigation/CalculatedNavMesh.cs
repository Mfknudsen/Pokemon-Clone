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

        [SerializeField] private Vector3[] vertices;
        [SerializeField] private NavTriangle[] triangles;
        [SerializeField] private int[] areaType;
        [SerializeField] private NavigationPoint[] navigationPoints;
        [SerializeField] private Dictionary<int, NavigationPointEntry[]> navigationEntryPoints;

        #endregion

        #region Build In States

        public void SetValues(Vector3[] vertices, NavTriangle[] triangles, int[] areaType, NavigationPoint[] navigationPoints, Dictionary<int, NavigationPointEntry[]> navigationEntryPoints)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.areaType = areaType;
            this.navigationPoints = navigationPoints;
            this.navigationEntryPoints = navigationEntryPoints;
        }

        #endregion

        #region Getters

        public Vector3[] Vertices => this.vertices;

        public NavTriangle[] Triangles => this.triangles;

        #endregion
    }

    [Serializable]
    public class NavTriangle
    {
        #region Values

        [SerializeField]
        private int A, B, C;
        [SerializeField]
        private int area;
        [SerializeField]
        private List<int> neighborIDs;
        [SerializeField]
        private List<int> navPointIDs;

        #endregion

        #region Build In States

        public NavTriangle(int A, int B, int C, int area)
        {
            this.A = A;
            this.B = B;
            this.C = C;

            this.area = area;

            this.neighborIDs ??= new();
            this.navPointIDs ??= new();
        }

        #endregion

        #region Getters

        public int[] Vertices => new int[] { this.A, this.B, this.C };

        public int[] Neighbors => this.neighborIDs.ToArray();

        public int[] NavPoints => this.navPointIDs.ToArray();

        public int Area => this.area;

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetNeighborIDs(List<int> set) => this.neighborIDs = set;

        public void SetNavPointIDs(List<int> set) => this.navPointIDs = set;
#endif

        #endregion

        #region Out

#if UNITY_EDITOR
        public Vector3 Center(Vector3[] vertices) => Vector3.Lerp(Vector3.Lerp(vertices[this.A], vertices[this.B], .5f), vertices[this.C], .5f);
#endif

        #endregion
    }
}