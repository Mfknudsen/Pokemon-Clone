#region Libraries

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation;
using Runtime.Core;
using UnityEngine;

#endregion

namespace Runtime.Algorithms.PathFinding
{
    /// <summary>
    /// Funnel algorithm use to after the a* algorithm.
    /// Used to get the quickest path along the a* triangle path with the fewest turns.
    /// </summary>
    public static class Funnel
    {
        /// <summary>
        /// Funnel algorithm to be used on an created path from the a* algorithm.
        /// </summary>
        /// <param name="start">Position of the agent</param>
        /// <param name="end">Target destination</param>
        /// <param name="triangleIDs">List of ids for the relevant triangles used in the a* path</param>
        /// <param name="triangles">List of triangles from the custom navmesh</param>
        /// <param name="verts">Array of 3D vertices from the custom navmesh</param>
        /// <param name="agent">The agent that requested the path</param>
        /// <returns></returns>
        public static List<Vector3> GetPath(Vector3 start, Vector3 end, int[] triangleIDs, NavTriangle[] triangles,
            Vector3[] verts, UnitAgent agent)
        {
            //Result list containing the different position for the agent to travel along.
            List<Vector3> result = new List<Vector3>();
            //List of portals to check.
            List<Portal> portals = GetPortals(start.XZ(), triangleIDs, triangles, verts, agent.Settings.Radius,
                out Vector2[] remappedSimpleVerts, out Vector3[] remappedVerts);

            //Apex is the newest point the agent will travel to.
            Vector2 apex = start.XZ(),
                //Portal vert ids is the current funnel.
                portalLeft = remappedSimpleVerts[portals[0].left],
                portalRight = remappedSimpleVerts[portals[0].right];

            //Ids to be used when setting the new apex and adding to the result.
            int leftID = portals[0].left,
                rightID = portals[0].right,
                //Used when resetting the for loop to the portal which the newest apex originates from.
                leftPortalID = 0,
                rightPortalID = 0;

            //Checking the portals.
            for (int i = 1; i < portals.Count + 1; i++)
            {
                //The newest points to be checked against the current funnel.
                Vector2 left = i < portals.Count ? remappedSimpleVerts[portals[i].left] : end.XZ(),
                    right = i < portals.Count ? remappedSimpleVerts[portals[i].right] : left;

                if (TriArea2(apex, portalRight, right) <= 0f)
                {
                    //Update right
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

                if (!(TriArea2(apex, portalLeft, left) >= 0f)) continue;
                //Update left

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

            if (result.Count == 0 || result[^1] != end)
                result.Add(end);

            return result;
        }

        /// <summary>
        /// Creates the portals in order of the triangles.
        /// Left and right of the portals is also properly set.
        /// </summary>
        /// <param name="start">The agents position. Used to determine left and right</param>
        /// <param name="triangleIDs">List of ids for the relevant triangles used in the path</param>
        /// <param name="triangles">List of triangles from the custom navmesh</param>
        /// <param name="verts">List of 3D vertices</param>
        /// <param name="agentRadius">The radius of the agent</param>
        /// <param name="remappedSimpleVerts">Out returns an array of remapped 2D vertices</param>
        /// <param name="remappedVerts">Out returns an array of remapped 3D vertices</param>
        /// <returns>List of portals in order from the agents start position to the target destination</returns>
        private static List<Portal> GetPortals(Vector2 start, IReadOnlyList<int> triangleIDs,
            IReadOnlyList<NavTriangle> triangles, IReadOnlyList<Vector3> verts, float agentRadius,
            out Vector2[] remappedSimpleVerts, out Vector3[] remappedVerts)
        {
            //RemappingVertices
            List<Vector3> remappedVertsResult = new List<Vector3>();
            List<Vector2> remappedSimpleVertsResult = new List<Vector2>();
            int[] shared;
            Dictionary<int, RemappedVert> remapped = new Dictionary<int, RemappedVert>();
            for (int i = 1; i < triangleIDs.Count; i++)
            {
                shared = triangles[triangleIDs[i]].Vertices.SharedBetween(triangles[triangleIDs[i - 1]].Vertices, 2);

                Vector3 ab = verts[shared[0]] - verts[shared[1]];

                if (remapped.TryGetValue(shared[0], out RemappedVert remappedVert))
                {
                    remappedVert.directionChange -= ab;
                    remapped[shared[0]] = remappedVert;
                }
                else
                    remapped.Add(shared[0],
                        new RemappedVert(remapped.Count, verts[shared[0]], -ab));

                if (remapped.TryGetValue(shared[1], out remappedVert))
                {
                    remappedVert.directionChange += ab;
                    remapped[shared[1]] = remappedVert;
                }
                else
                    remapped.Add(shared[1],
                        new RemappedVert(remapped.Count, verts[shared[1]], ab));
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
                new Portal(remapped[shared[MathC.IsPointLeftToVector(start, forwardEnd, remappedSimpleVerts[0])
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


        /// <summary>
        /// Calculates if clockwise or counter clockwise
        /// </summary>
        /// <param name="a">Apex</param>
        /// <param name="b">Portal point</param>
        /// <param name="c">New point</param>
        /// <returns>Returns positive value if clockwise and negative value if counter clockwise</returns>
        private static float TriArea2(Vector2 a, Vector2 b, Vector2 c)
        {
            float ax = b.x - a.x;
            float ay = b.y - a.y;
            float bx = c.x - a.x;
            float by = c.y - a.y;
            return bx * ay - ax * by;
        }

        /// <summary>
        /// Quick distance point between two points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>True if the square distance between the two points are less then 0.01</returns>
        private static bool VEqual(Vector2 a, Vector2 b) =>
            (a - b).sqrMagnitude < 0.1f * 0.1f;
    }

    /// <summary>
    /// Portal to be created between each triangle with the correct left and right compared to the position of the agent.
    /// </summary>
    internal readonly struct Portal
    {
        /// <summary>
        /// Vertices id to be used with the remapped vertices list.
        /// </summary>
        [NonSerialized] public readonly int left, right;

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

    /// <summary>
    /// Used to remap the vertices from the custom navmesh to match the agents radius.
    /// Remapping will insure the agent don't hit things like buildings while traveling the path.
    /// </summary>
    internal struct RemappedVert
    {
        /// <summary>
        /// The id of the vertices for the remapped vertices.
        /// This struct will be placed in a dictionary with the previous id as the key.
        /// </summary>
        [NonSerialized] public readonly int newID;

        [NonSerialized] public Vector3 vert;
        [NonSerialized] public Vector2 simpleVert;

        [NonSerialized] public Vector3 directionChange;

        public RemappedVert(int newID, Vector3 vert, Vector3 directionChange)
        {
            this.newID = newID;
            this.vert = vert;
            this.simpleVert = Vector2.zero;
            this.directionChange = directionChange;
        }

        /// <summary>
        /// After all the remapped vertices have been created then set the offset vert and a 2D version of it.
        /// </summary>
        /// <param name="agentRadius"></param>
        public void Set(float agentRadius)
        {
            this.vert += this.directionChange.normalized * agentRadius;
            this.simpleVert = this.vert.XZ();
        }
    }
}