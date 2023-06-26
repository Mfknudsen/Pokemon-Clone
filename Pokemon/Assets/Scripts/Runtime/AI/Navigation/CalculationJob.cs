#region Libraries

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

#endregion

namespace Runtime.AI.Navigation
{
    public struct PathCalculatorJob : IJobParallelFor, IDisposable
    {
        #region Values

        [WriteOnly] private NativeArray<JobPath> paths;
        [ReadOnly] private NativeArray<JobAgent> agents;

        [ReadOnly] private NativeArray<float3> verts;
        [ReadOnly] private NativeArray<float2> simpleVerts;
        [ReadOnly] private NativeArray<int> areas;

        [ReadOnly] private NativeArray<JobTriangle> triangles;

        UnsafeList<Node> toCheckNodes, checkedNodes;

        #endregion

        #region Build In States

        public PathCalculatorJob(NativeArray<JobPath> paths, NativeArray<JobAgent> agents, NativeArray<float3> verts, NativeArray<float2> simpleVerts,
           NativeArray<int> areas, NativeArray<JobTriangle> triangles)
        {
            this.paths = paths;
            this.agents = agents;
            this.verts = verts;
            this.simpleVerts = simpleVerts;
            this.areas = areas;
            this.triangles = triangles;

            this.toCheckNodes = new UnsafeList<Node>(triangles.Length, Allocator.TempJob);
            this.checkedNodes = new UnsafeList<Node>(triangles.Length, Allocator.TempJob);
        }

        public void Execute(int index)
        {
            float3 destination = this.agents[index].desination;
            this.toCheckNodes.Add(new Node(this.triangles[index], this.simpleVerts, new float2(destination.x, destination.z), 0, this.areas));

            int i = 0;
            while (this.toCheckNodes.Length > 0)
            {
                i++;
                if (i == this.triangles.Length)
                    break;

                Node checking = this.toCheckNodes[0];
                this.checkedNodes.Add(checking);
                this.toCheckNodes.RemoveAt(0);
                if (checking.triangleID == this.agents[index].destinationTriangleID)
                {
                    this.paths[index] = new JobPath(checking, this.checkedNodes);
                    break;
                }

                this.toCheckNodes = this.AddNeighbors(this.checkedNodes, this.toCheckNodes, this.triangles[checking.triangleID].neighbors, checking, this.triangles, this.verts, this.areas, this.agents[index]);
            }
        }

        public void Dispose()
        {
            if (this.toCheckNodes.IsCreated)
                this.toCheckNodes.Dispose();
            if (this.checkedNodes.IsCreated)
                this.checkedNodes.Dispose();
        }

        #endregion

        #region Internal

        private UnsafeList<Node> AddNeighbors(UnsafeList<Node> checkedNodes, UnsafeList<Node> toCheck, UnsafeList<int> toAdd, Node original, NativeArray<JobTriangle> triangles, NativeArray<float3> verts, NativeArray<int> areas, JobAgent agent)
        {
            for (int i = 0; i < toAdd.Length; i++)
                toCheck.Add(new Node());

            UnsafeList<Node> result = new(toCheck.Length + toAdd.Length, Allocator.Temp);
            for (int i = 0; i < toCheck.Length; i++)
                result.Add(toCheck[i]);

            for (int i = 0; i < toAdd.Length; i++)
            {
                bool exists = false;
                for (int j = 0; j < checkedNodes.Length; j++)
                {
                    if (checkedNodes[j].triangleID != toAdd[i])
                        continue;

                    exists = true;
                    break;
                }

                if (!exists)
                {
                    for (int j = 0; j < toCheck.Length; j++)
                    {
                        if (toCheck[j].triangleID != toAdd[i])
                            continue;

                        exists = true;
                        break;
                    }
                }

                if (exists)
                    continue;

                float lowestCost = original.cost;
                int lowestID = -1;
                for (int j = 0; j < checkedNodes.Length; j++)
                {
                    if (!this.Contains(triangles[toAdd[i]].neighbors, checkedNodes[j].triangleID))
                        continue;

                    if (checkedNodes[j].cost > lowestCost)
                        continue;

                    lowestID = j;
                }

                Node lowest = lowestID == -1 ? original : checkedNodes[lowestID];
                for (int j = 0; j < result.Length; j++)
                {
                    if (result[j].cost > lowest.cost)
                    {
                        result.Add(new Node(triangles[toAdd[i]], this.simpleVerts, new float2(agent.desination.x, agent.desination.z), lowest, areas));
                        break;
                    }
                }
            }

            toCheck.Dispose();

            return result;
        }

        private bool Contains(UnsafeList<int> list, int target)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i] == target)
                    return true;

            return false;
        }

        #endregion
    }

    public readonly struct JobPath : IDisposable
    {
        public readonly UnsafeList<Node> nodePath;
        private readonly UnsafeList<Node> checkList;

        public JobPath(Node lastNode, UnsafeList<Node> checkedNodes)
        {
            Node checking = lastNode;
            this.checkList = new UnsafeList<Node>(checkedNodes.Length, Allocator.Temp);
            int breaker = 0;
            JobLogger.Log(checkedNodes.Length);
            while (checking.triangleID != -1)
            {
                breaker++;
                if (breaker == checkedNodes.Length)
                    break;

                this.checkList.Add(checking);

                if (checking.previousNode == -1)
                    break;

                for (int i = 0; i < checkedNodes.Length; i++)
                {
                    if (checkedNodes[i].triangleID == checking.previousNode)
                    {
                        checking = checkedNodes[i];
                        break;
                    }
                }
            }

            this.nodePath = new UnsafeList<Node>(this.checkList.Length, Allocator.TempJob);
            for (int i = 0; i < this.checkList.Length; i++)
                this.nodePath.Add(this.checkList[i]);

            this.checkList.Dispose();
        }

        public void Dispose()
        {
            if (this.nodePath.IsCreated)
                this.nodePath.Dispose();
            if (this.checkList.IsCreated)
                this.checkList.Dispose();
        }
    }

    public readonly struct JobAgent
    {
        #region Values

        public readonly int currentTriangleID, destinationTriangleID;

        public readonly float3 desination;

        #endregion

        #region Build In States

        public JobAgent(QueuedAgentRequest request)
        {
            UnitNavigationAgent agent = request.agent;
            UnitAgentSettings settings = agent.Settings;

            this.currentTriangleID = agent.CurrentTriangleIndex;
            this.desination = request.destination;
            this.destinationTriangleID = UnitNavigation.ClosestTriangleIndex(this.desination);
        }

        #endregion
    }

    public readonly struct JobTriangle : IDisposable
    {
        #region Values

        public readonly int id, a, b, c, area;

        public readonly UnsafeList<int> neighbors, navPoints;

        public readonly UnsafeList<float> widths;

        #endregion

        #region Build In States

        public JobTriangle(NavTriangle triangle)
        {
            this.id = triangle.ID;
            this.a = triangle.Vertices[0];
            this.b = triangle.Vertices[1];
            this.c = triangle.Vertices[2];
            this.area = triangle.Area;

            this.neighbors = new UnsafeList<int>(triangle.Neighbors.Count, Allocator.Temp);
            for (int i = 0; i < triangle.Neighbors.Count; i++)
                this.neighbors.Add(triangle.Neighbors[i]);

            this.navPoints = new UnsafeList<int>(triangle.NavPoints.Count, Allocator.Temp);
            for (int i = 0; i < triangle.NavPoints.Count; i++)
                this.navPoints.Add(triangle.NavPoints[i]);

            this.widths = new UnsafeList<float>(triangle.Widths.Count, Allocator.Temp);
            for (int i = 0; i < triangle.Widths.Count; i++)
                this.widths.Add(triangle.Widths[i]);
        }

        public void Dispose()
        {
            this.neighbors.Dispose();
            this.navPoints.Dispose();
            this.widths.Dispose();
        }

        #endregion
    }

    public readonly struct Node
    {
        public readonly int triangleID;
        public readonly float cost, dist;
        public readonly int previousNode;

        public Node(JobTriangle triangle, NativeArray<float2> verts, float2 destination, Node previousNode, NativeArray<int> areas)
        {
            this.previousNode = previousNode.triangleID;
            this.triangleID = triangle.id;
            this.cost = previousNode.cost + areas[triangle.id] + 1;
            this.dist = math.min(
                math.distance(destination, verts[triangle.a]),
                    math.min(
                        math.distance(destination, verts[triangle.b]),
                        math.distance(destination, verts[triangle.c])));

        }

        public Node(JobTriangle triangle, NativeArray<float2> simpleVerts, float2 destination, float previousNode, NativeArray<int> areas)
        {
            this.previousNode = -1;
            this.triangleID = triangle.id;
            this.cost = previousNode + areas[triangle.id] + 1;
            this.dist = math.min(
                math.distance(destination, simpleVerts[triangle.a]),
                    math.min(
                        math.distance(destination, simpleVerts[triangle.b]),
                        math.distance(destination, simpleVerts[triangle.c])));

        }
    }
}