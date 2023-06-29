#region Libraries

using Runtime.Common;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    public struct UnitPath
    {
        #region Values

        private int actionIndex;
        private readonly List<PathAction> actions;

        private readonly Vector3 destination;

        private readonly int[] ids;

        #endregion

        #region Build In States

        public UnitPath(Vector3 destination, int[] pathTriangleIDs, NavTriangle[] triangles, Vector3[] verts, Vector3 startPoint, float agentRadius)
        {
            this.ids = new int[pathTriangleIDs.Length];

            this.actionIndex = 0;
            this.destination = destination;

            this.actions = new();
            Vector2 closestPoint = startPoint.XZ();
            for (int i = pathTriangleIDs.Length - 2; i >= 0; i--)
            {
                if (triangles[pathTriangleIDs[i]].Neighbors.Contains(pathTriangleIDs[i + 1]))
                {
                    int[] indsBetween = triangles[pathTriangleIDs[i]].Vertices
                        .SharedBetween(triangles[pathTriangleIDs[i + 1]].Vertices);

                    Vector2 dir = (verts[indsBetween[1]].XZ() - verts[indsBetween[0]].XZ()).normalized;
                    Vector2 p = ExtMathf.ClosetPointOnLine(closestPoint + dir.Cross(),
                        verts[indsBetween[0]].XZ() + dir * (agentRadius * 1.2f),
                        verts[indsBetween[1]].XZ() - dir * (agentRadius * 1.2f));

                    closestPoint = p;

                    this.actions.Add(new WalkAction(p.ToV3(triangles[pathTriangleIDs[i]].MaxY)));
                }
                else
                    this.actions.Add(new InteractAction());
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
        public void DebugPath()
        {
            if (this.Empty || !UnitNavigation.Ready)
                return;

            for (int i = 0; i < this.actions.Count; i++)
                Debug.DrawRay((this.actions[i] as WalkAction).destination, Vector3.up, Color.yellow);

            /*
            Vector3[] verts = UnitNavigation.GetVerts();

            for (int i = 1; i < this.ids.Length; i++)
            {
                NavTriangle t1 = UnitNavigation.GetTriangleByID(i - 1), t2 = UnitNavigation.GetTriangleByID(i);
                Debug.DrawLine(t1.Center(verts) + Vector3.up, t2.Center(verts) + Vector3.up);
            }
            */
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