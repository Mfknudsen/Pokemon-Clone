#region Libraries

using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation;
using Runtime.Common;
using Runtime.Common.CommonMathf;
using UnityEngine;

#endregion

namespace Runtime.Algorithms
{
    public static class SSFunnelAlgorithm
    {
        public static List<Vector3> GetPositionsFromEdges(Vector3 startPoint, Vector3 endPoint, Vector2[] simpleVerts,
            Vector3[] verts, List<NavTriangle> currentWalkablePath)
        {
            List<Vector3> result = new();
            List<Portal> portals = TrianglesToPortals(currentWalkablePath, verts);

            Vector3 apex = startPoint;

            for (int i = 0; i < portals.Count; i++)
            {
                Portal toInsert = portals[i];

                bool addRight = toInsert.left == portals[i - 1].left;
                Vector3 p = addRight ? toInsert.right : toInsert.left;

                if (addRight)
                    AddRight(p);
                else
                    AddLeft(p);
            }

            return result;
        }

        private static void AddLeft(Vector3 v)
        {
        }

        private static void AddRight(Vector3 v)
        {
        }

        public static List<Edge> GetEdgesFromTrianglePath(List<NavTriangle> triangles, Vector3[] verts)
        {
            List<Edge> result = new();

            int[] previousShared = triangles[0].Vertices.SharedBetween(triangles[1].Vertices);

            for (int i = 1; i < triangles.Count - 1; i++)
            {
                int[] currentShared = triangles[i].Vertices.SharedBetween(triangles[i + 1].Vertices);

                result.Add(new Edge(verts[previousShared.First(id => !currentShared.Contains(id))],
                    verts[currentShared.First(id => !previousShared.Contains(id))]));

                previousShared = currentShared;
            }

            return result;
        }

        public static List<Edge> GetSideEdgesByStartIndex(Vector3 startPoint, Vector3 firstEdgeConnection,
            List<Edge> allEdges)
        {
            List<Edge> result = new() { new Edge(startPoint, firstEdgeConnection) };

            Vector3 newest = firstEdgeConnection;

            bool found = true;
            int limit = 0;

            while (found && limit < allEdges.Count)
            {
                limit++;
                found = false;

                for (int i = 0; i < allEdges.Count; i++)
                {
                    if (result.Contains(allEdges[i]))
                        continue;

                    if (allEdges[i].Contains(newest))
                    {
                        result.Add(allEdges[i]);
                        newest = allEdges[i].Other(newest);
                        found = true;

                        break;
                    }
                }
            }

            return result;
        }

        private static List<Portal> TrianglesToPortals(List<NavTriangle> triangles, Vector3[] verts)
        {
            List<Portal> result = new List<Portal>();

            int[] previousShared = triangles[0].Vertices.SharedBetween(triangles[1].Vertices), nextShared;
            result.Add(new Portal(verts[previousShared[0]],
                verts[previousShared[1]]));
            int previousLeft = previousShared[1];

            for (int i = 1; i < triangles.Count; i++)
            {
                nextShared = triangles[i].Vertices.SharedBetween(triangles[i + 1].Vertices);

                if (nextShared.Contains(previousLeft))
                {
                    if (nextShared[0] == previousLeft)
                        result.Add(new Portal(
                            verts[nextShared[1]],
                            verts[nextShared[0]]));
                    else
                        result.Add(new Portal(
                            verts[nextShared[0]],
                            verts[nextShared[1]]));
                }
                else
                {
                    if (previousShared.Contains(nextShared[0]))
                    {
                        previousLeft = nextShared[1];
                        result.Add(new Portal(
                            verts[nextShared[0]],
                            verts[nextShared[1]]));
                    }
                    else
                    {
                        previousLeft = nextShared[0];
                        result.Add(new Portal(
                            verts[nextShared[1]],
                            verts[nextShared[0]]));
                    }
                }

                previousShared = nextShared;
            }

            return result;
        }
    }

    internal readonly struct Portal
    {
        public readonly Vector3 right, left;

        public Portal(Vector3 right, Vector3 left)
        {
            this.right = right;
            this.left = left;
        }
    }
}