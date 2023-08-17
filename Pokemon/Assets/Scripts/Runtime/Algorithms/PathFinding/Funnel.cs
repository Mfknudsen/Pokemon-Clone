#region Libraries

using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation;
using Runtime.Core;
using UnityEngine;

#endregion

namespace Runtime.Algorithms.PathFinding
{
    public static class Funnel
    {
        public static List<Vector3> GetPath(Vector3 start, Vector3 end, int[] triangleIDs, NavTriangle[] triangles,
            Vector3[] verts, UnitAgent agent, out List<Portal> portals,
            out Dictionary<int, RemappedVert> t)
        {
            List<Vector3> result = new List<Vector3>();
            portals = GetGates(start.XZ(), triangleIDs, triangles, verts, agent.Settings.Radius,
                out Vector2[] remappedSimpleVerts, out Vector3[] remappedVerts,
                out Dictionary<int, RemappedVert> remapped);
            agent.portals = portals;
            agent.remappedVerts = remapped;
            t = remapped;
            Vector2 apex = start.XZ();
            Vector2 portalLeft =
                remappedSimpleVerts[portals[0].left];
            Vector2 portalRight =
                remappedSimpleVerts[portals[0].right];

            int leftID = portals[0].left;
            int rightID = portals[0].right;
            int leftPortalID = 0;
            int rightPortalID = 0;

            for (int i = 1; i < portals.Count + 1; i++)
            {
                Vector2 left = i < portals.Count ? remappedSimpleVerts[portals[i].left] : end.XZ();
                Vector2 right = i < portals.Count ? remappedSimpleVerts[portals[i].right] : left;

                //Update right
                if (TriArea2(apex, portalRight, right) <= 0f)
                {
                    if (VEqual(apex, portalRight) ||
                        TriArea2(apex, portalLeft, right) > 0f)
                    {
                        portalRight = right;
                        rightPortalID = i;

                        if (i < portals.Count)
                            rightID = portals[i].right;
                    }
                    else
                    {
                        result.Add(i < portals.Count ? remappedVerts[leftID] : end);

                        apex = remappedSimpleVerts[leftID];
                        rightID = leftID;
                        portalLeft = apex;
                        portalRight = apex;
                        i = leftPortalID;
                        continue;
                    }
                }

                //Update left
                if (TriArea2(apex, portalLeft, left) >= 0f)
                {
                    if (VEqual(apex, portalLeft) ||
                        TriArea2(apex, portalRight, left) < 0f)
                    {
                        portalLeft = left;
                        leftPortalID = i;

                        if (i < portals.Count)
                            leftID = portals[i].left;
                    }
                    else
                    {
                        result.Add(i < portals.Count ? remappedVerts[rightID] : end);

                        apex = remappedSimpleVerts[rightID];
                        leftID = rightID;
                        portalLeft = apex;
                        portalRight = apex;
                        i = rightPortalID;
                    }
                }
            }

            if (result.Count == 0 || result[^1] != end)
                result.Add(end);

            Debug.Log("R: " + result.Count);
            return result;
        }

        private static List<Portal> GetGates(Vector2 start, IReadOnlyList<int> triangleIDs,
            IReadOnlyList<NavTriangle> triangles, IReadOnlyList<Vector3> verts, float agentRadius,
            out Vector2[] remappedSimpleVerts, out Vector3[] remappedVerts, out Dictionary<int, RemappedVert> remapped)
        {
            //RemappingVertices
            List<Vector3> remappedVertsResult = new List<Vector3>();
            List<Vector2> remappedSimpleVertsResult = new List<Vector2>();
            int[] shared;
            remapped = new Dictionary<int, RemappedVert>();
            for (int i = 1; i < triangleIDs.Count; i++)
            {
                shared = triangles[triangleIDs[i]].Vertices.SharedBetween(triangles[triangleIDs[i - 1]].Vertices, 2);

                Vector3 betweenNorm = verts[shared[0]] - verts[shared[1]];

                if (remapped.TryGetValue(shared[0], out RemappedVert remappedVert))
                {
                    remappedVert.directionChange -= betweenNorm;
                    remapped[shared[0]] = remappedVert;
                }
                else
                    remapped.Add(shared[0],
                        new RemappedVert(remapped.Count, verts[shared[0]], -betweenNorm));

                if (remapped.TryGetValue(shared[1], out remappedVert))
                {
                    remappedVert.directionChange += betweenNorm;
                    remapped[shared[1]] = remappedVert;
                }
                else
                    remapped.Add(shared[1],
                        new RemappedVert(remapped.Count, verts[shared[1]], betweenNorm));
            }

            int[] key = remapped.Keys.ToArray();
            for (int i = 0; i < remapped.Count; i++)
            {
                RemappedVert remappedVert = remapped[key[i]];
                remappedVert.Set(agentRadius);
                remappedVertsResult.Add(remappedVert.vert);
                remappedSimpleVertsResult.Add(remappedVert.simpleVert);
                remapped[key[i]] = remappedVert;
            }

            remappedVerts = remappedVertsResult.ToArray();
            remappedSimpleVerts = remappedSimpleVertsResult.ToArray();

            //Creating portals
            shared = triangles[triangleIDs[0]].Vertices.SharedBetween(triangles[triangleIDs[1]].Vertices, 2);
            Vector2 forwardEnd = remappedSimpleVerts[remapped[shared[0]].newID] +
                                 (remappedSimpleVerts[remapped[shared[1]].newID] -
                                  remappedSimpleVerts[remapped[shared[0]].newID]) * .5f;
            List<Portal> result = new List<Portal>
            {
                new Portal(remapped[shared[MathC.isPointLeftToVector(start, forwardEnd, remappedSimpleVerts[0])
                        ? 0
                        : 1]].newID,
                    -1, remapped[shared[0]].newID, remapped[shared[1]].newID)
            };

            for (int i = 1; i < triangleIDs.Count - 1; i++)
            {
                shared = triangles[triangleIDs[i]].Vertices.SharedBetween(triangles[triangleIDs[i + 1]].Vertices, 2);
                result.Add(new Portal(result[^1].left, result[^1].right,
                    remapped[shared[0]].newID, remapped[shared[1]].newID));
            }

            return result;
        }

        private static float TriArea2(Vector2 a, Vector2 b, Vector2 c)
        {
            float ax = b.x - a.x;
            float ay = b.y - a.y;
            float bx = c.x - a.x;
            float by = c.y - a.y;
            return bx * ay - ax * by;
        }

        private static bool VEqual(Vector2 a, Vector2 b) =>
            (a - b).sqrMagnitude < 0.1f * 0.1f;
    }

    public readonly struct Portal
    {
        public readonly int left, right;

        public Portal(int previousLeft, int previousRight, int a, int b)
        {
            if (previousLeft == a || previousRight == b)
            {
                this.left = a;
                this.right = b;
            }
            else
            {
                this.left = b;
                this.right = a;
            }
        }
    }

    public struct RemappedVert
    {
        public readonly int newID;

        public Vector3 vert;
        public Vector2 simpleVert;

        public Vector3 directionChange;

        public RemappedVert(int newID, Vector3 vert, Vector3 directionChange)
        {
            this.newID = newID;
            this.vert = vert;
            this.simpleVert = Vector2.zero;
            this.directionChange = directionChange;
        }

        public void Set(float agentRadius)
        {
            this.vert += this.directionChange.normalized * agentRadius;
            this.simpleVert = this.vert.XZ();
        }
    }
}