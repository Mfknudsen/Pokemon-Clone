#region Libraries

using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation.PathActions;
using Runtime.Algorithms;
using Runtime.Common;
using Runtime.Common.CommonMathf;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    [System.Serializable]
    public struct UnitPath
    {
        #region Values

        [ShowInInspector] private int actionIndex;
        [ShowInInspector] private readonly List<PathAction> actions;

        [ShowInInspector] private readonly Vector3 destination;
        [ShowInInspector] private List<NavTriangle> pathTriangles;
        [ShowInInspector] private Vector3[] verts;

        [ShowInInspector] private readonly int[] ids;

        private List<Edge> edges;
        private List<Edge> left, right;

        #endregion

        #region Build In States

        public UnitPath(Vector3 destination, int[] pathTriangleIDs, NavTriangle[] triangles, Vector3[] verts,
            Vector2[] simpleVerts, Vector3 startPoint, UnitAgent agent)
        {
            this.verts = verts;
            this.pathTriangles = new List<NavTriangle>();
            foreach (int pathTriangleID in pathTriangleIDs.Reverse())
                this.pathTriangles.Add(triangles[pathTriangleID]);

            this.edges =
                SSFunnelAlgorithm.GetEdgesFromTrianglePath(this.pathTriangles, verts);
            int[] startShared = this.pathTriangles[0].Vertices.SharedBetween(this.pathTriangles[1].Vertices);
            Vector3 position = agent.transform.position;
            this.left = SSFunnelAlgorithm.GetSideEdgesByStartIndex(position, this.verts[startShared[0]],
                this.edges);
            this.right = SSFunnelAlgorithm.GetSideEdgesByStartIndex(position, this.verts[startShared[1]],
                this.edges);

            this.ids = new int[pathTriangleIDs.Length];

            this.actionIndex = 0;
            this.destination = destination;

            this.actions = new List<PathAction>();

            int[] correctedPath = pathTriangleIDs.Reverse().ToArray();
            List<NavTriangle> currentWalkablePath = new List<NavTriangle>();
            for (int i = 0; i < correctedPath.Length - 1; i++)
            {
                currentWalkablePath.Add(triangles[correctedPath[i]]);

                if (!triangles[correctedPath[i]].Neighbors.Contains(triangles[correctedPath[i + 1]].ID))
                {
                    foreach (Vector3 p in SSFunnelAlgorithm.GetPositionsFromEdges(startPoint, destination, simpleVerts,
                                 verts, currentWalkablePath))
                        this.actions.Add(new WalkAction(p));

                    currentWalkablePath.Clear();
                }
            }

            if (currentWalkablePath.Count != 0)
            {
                foreach (Vector3 p in SSFunnelAlgorithm.GetPositionsFromEdges(startPoint, this.destination, simpleVerts,
                             verts, currentWalkablePath))
                    this.actions.Add(new WalkAction(p));
            }

            this.actions.Add(new WalkAction(destination));
        }

        #endregion

        #region Getters

        public readonly Vector3 Destination() => this.destination;

        #endregion

        #region Out

        public readonly bool Empty => this.actions == null || this.actions.Count == 0;

        public readonly bool Complete => this.actions != null && this.actions.Count == this.actionIndex;

        #endregion

        #region In

#if UNITY_EDITOR
        public void DebugPath(UnitAgent agent)
        {
            if (this.pathTriangles != null && this.pathTriangles.Count > 0)
            {
                NavTriangle previous = this.pathTriangles[0];
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.blue;

                Debug.DrawLine(previous.Center(this.verts), agent.transform.position, Color.red);

                for (int index = 1; index < this.pathTriangles.Count; index++)
                {
                    Debug.DrawLine(previous.Center(this.verts), this.pathTriangles[index].Center(this.verts),
                        Color.red);
                    previous = this.pathTriangles[index];
                    NavTriangle pathTriangle = this.pathTriangles[index];
                    Handles.Label(pathTriangle.Center(this.verts), pathTriangle.ID.ToString(), style);
                }

                foreach (Edge edge in this.edges)
                    Debug.DrawLine(edge.A + Vector3.up, edge
                        .B + Vector3.up, Color.yellow);

                foreach (Edge edge in this.left)
                    Debug.DrawLine(edge.A + (Vector3.up * 2), edge.B + (Vector3.up * 2), Color.magenta);

                foreach (Edge edge in this.right)
                    Debug.DrawLine(edge.A + (Vector3.up * 2), edge.B + (Vector3.up * 2), Color.green);
            }

            if (this.Empty || !UnitNavigation.Ready)
                return;

            for (int i = 0; i < this.actions.Count; i++)
                Debug.DrawRay(this.actions[i].Destination(), Vector3.up, Color.yellow);
        }
#endif

        public void Tick(UnitAgent agent)
        {
            if (this.actions[this.actionIndex].PerformAction(agent))
                this.actionIndex++;
        }

        #endregion
    }
}