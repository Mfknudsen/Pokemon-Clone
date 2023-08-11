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

        [ShowInInspector] private readonly Vector3 destination;
        [ShowInInspector] private List<NavTriangle> pathTriangles;
        [ShowInInspector] private Vector3[] verts;

        #endregion

        #region Build In States

        public UnitPath(Vector3 destination, int[] pathTriangleIDs, NavTriangle[] triangles, Vector3[] verts,
            Vector2[] simpleVerts, Vector3 startPoint, UnitAgent agent)
        {
            this.actionIndex = 0;
            this.destination = destination;
            this.actions = new List<PathAction>();
            this.verts = verts;
            this.pathTriangles = new List<NavTriangle>();
            foreach (int pathTriangleID in pathTriangleIDs.Reverse())
                this.pathTriangles.Add(triangles[pathTriangleID]);

            foreach (Vector3 point in Funnel.GetPath())
            {
                this.actions.Add(new WalkAction(point));
            }
        }

        #endregion

        #region Getters

        public readonly Vector3 Destination() => this.destination;

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