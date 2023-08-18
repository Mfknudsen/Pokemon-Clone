#region Libraries

using System.Collections.Generic;
using Runtime.AI.Navigation.PathActions;
using Runtime.Algorithms.PathFinding;
using Sirenix.OdinInspector;
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

        #endregion

        #region Build In States

        public UnitPath(Vector3 startPoint, Vector3 endPoint, int[] pathTriangleIDs, NavTriangle[] triangles,
            Vector3[] verts,
            Vector2[] simpleVerts, UnitAgent agent)
        {
            this.actionIndex = 0;
            this.endPoint = endPoint;
            this.actions = new List<PathAction>();

            foreach (Vector3 point in Funnel.GetPath(startPoint, endPoint, pathTriangleIDs, triangles, verts, agent))
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

            Debug.DrawLine(agent.transform.position, this.actions[0].Destination(), Color.yellow);
            for (int i = 1; i < this.actions.Count; i++)
                Debug.DrawLine(this.actions[i].Destination(), this.actions[i - 1].Destination(), Color.yellow);
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