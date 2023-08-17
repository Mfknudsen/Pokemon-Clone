#region Libraries

using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation.PathActions;
using Runtime.Algorithms.PathFinding;
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

        [ShowInInspector] private readonly Vector3 endPoint;
        [ShowInInspector] private List<NavTriangle> pathTriangles;
        [ShowInInspector] private Vector3[] verts;

        #endregion

        #region Build In States

        public UnitPath(Vector3 startPoint, Vector3 endPoint, int[] pathTriangleIDs, NavTriangle[] triangles,
            Vector3[] verts,
            Vector2[] simpleVerts, UnitAgent agent)
        {
            this.actionIndex = 0;
            this.endPoint = endPoint;
            this.actions = new List<PathAction>();
            this.verts = verts;
            this.pathTriangles = new List<NavTriangle>();
            foreach (int pathTriangleID in pathTriangleIDs.Reverse())
                this.pathTriangles.Add(triangles[pathTriangleID]);

            foreach (Vector3 point in Funnel.GetPath(startPoint, endPoint, pathTriangleIDs, triangles, verts, agent,
                         out List<Portal> temp, out Dictionary<int, RemappedVert> t))
                this.actions.Add(new WalkAction(point));
        }

        #endregion

        #region Getters

        public readonly Vector3 Destination() => this.endPoint;

        public readonly int ActionIndex() => this.actionIndex;

        #endregion

        #region Out

        public readonly bool Empty => this.actions == null || this.actions.Count == 0;

        public readonly bool Complete => this.actions != null && this.actions.Count == this.actionIndex;

        #endregion

        #region In

#if UNITY_EDITOR
        public void DebugPath(UnitAgent agent)
        {
            if (this.Empty || !UnitNavigation.Ready)
                return;

            if (this.pathTriangles is { Count: > 0 })
            {
                NavTriangle previous = this.pathTriangles[0];
                GUIStyle style = new GUIStyle
                {
                    normal = { textColor = Color.blue }
                };

                for (int index = 1; index < this.pathTriangles.Count; index++)
                {
                    Debug.DrawLine(previous.Center(this.verts), this.pathTriangles[index].Center(this.verts),
                        Color.red);
                    previous = this.pathTriangles[index];
                    NavTriangle pathTriangle = this.pathTriangles[index];
                    Handles.Label(pathTriangle.Center(this.verts), pathTriangle.ID.ToString(), style);
                }
            }

            Debug.DrawRay(this.actions[0].Destination(), Vector3.up, Color.green);
            for (int i = 1; i < this.actions.Count; i++)
            {
                Debug.DrawRay(this.actions[i].Destination(), Vector3.up, Color.green);
                Debug.DrawLine(this.actions[i].Destination(), this.actions[i - 1].Destination(), Color.yellow);
            }

            Vector3[] pos = agent.remappedVerts.Values.Select(r => r.vert).ToArray();
            for (int i = 0; i < agent.portals.Count - 1; i++)
            {
                Debug.DrawLine(pos[agent.portals[i].left] + Vector3.up,
                    pos[agent.portals[i + 1].left] + Vector3.up,
                    Color.blue);
                Debug.DrawLine(pos[agent.portals[i].right] + Vector3.up,
                    pos[agent.portals[i + 1].right] + Vector3.up,
                    Color.cyan);
            }
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