#region Libraries

using System;
using Runtime.Core;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

#endregion

namespace Runtime.AI.Navigation
{
    public struct AStartCalculationJob : IJobParallelFor, IDisposable
    {
        #region Values

        [WriteOnly] private NativeArray<JobPath> paths;
        [ReadOnly] private NativeArray<JobAgent> agents;

        [ReadOnly] private NativeArray<float2> simpleVerts;
        [ReadOnly] private NativeArray<int> areas;

        [ReadOnly] private NativeArray<JobTriangle> triangles;

        private UnsafeList<Node> toCheckNodes, checkedNodes;

        #endregion

        #region Build In States

        public AStartCalculationJob(NativeArray<JobPath> paths, NativeArray<JobAgent> agents,
            NativeArray<float2> simpleVerts,
            NativeArray<int> areas, NativeArray<JobTriangle> triangles)
        {
            this.paths = paths;
            this.agents = agents;
            this.simpleVerts = simpleVerts;
            this.areas = areas;
            this.triangles = triangles;

            this.toCheckNodes = new UnsafeList<Node>(triangles.Length, Allocator.TempJob);
            this.checkedNodes = new UnsafeList<Node>(triangles.Length, Allocator.TempJob);
        }

        public void Execute(int index)
        {
            float3 destination = this.agents[index].endPoint;
            this.toCheckNodes.Add(new Node(this.triangles[this.agents[index].currentTriangleID],
                this.simpleVerts, new float2(destination.x, destination.z), 0));

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
                    this.paths[index] = new JobPath(checking, this.checkedNodes, this.triangles);
                    break;
                }

                this.toCheckNodes = this.AddNeighbors(this.checkedNodes, this.toCheckNodes,
                    this.triangles[checking.triangleID].neighbors, checking, this.triangles,
                    this.areas, this.agents[index]);
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

        private readonly UnsafeList<Node> AddNeighbors(UnsafeList<Node> checkedNodes, UnsafeList<Node> toCheck,
            UnsafeList<int> toAddTriangleID, Node original, NativeArray<JobTriangle> triangles,
            NativeArray<int> areas, JobAgent agent)
        {
            UnsafeList<Node> result = new UnsafeList<Node>(toCheck.Length, Allocator.TempJob);
            for (int k = 0; k < toCheck.Length; k++)
                result.Add(toCheck[k]);

            for (int i = 0; i < toAddTriangleID.Length; i++)
            {
                if (toAddTriangleID[i] < 0 || toAddTriangleID[i] >= triangles.Length)
                    continue;

                if (triangles[original.triangleID].widths[i] < agent.radius * 2f)
                    continue;

                if (this.ContainsID(checkedNodes, toAddTriangleID[i]) || this.ContainsID(toCheck, toAddTriangleID[i]))
                    continue;

                Node bestPrevious = original;
                int checkedCount = 0;
                for (int z = 0; z < checkedNodes.Length; z++)
                {
                    if (this.ContainsID(triangles[toAddTriangleID[i]].neighbors, checkedNodes[z].triangleID))
                    {
                        checkedCount++;

                        if (checkedNodes[z].cost < bestPrevious.cost)
                            bestPrevious = checkedNodes[z];
                    }

                    if (checkedCount == triangles[toAddTriangleID[i]].neighbors.Length)
                        break;
                }

                Node newNode = new Node(triangles[toAddTriangleID[i]], this.simpleVerts, agent.endPoint.XZFloat(),
                    bestPrevious, areas, triangles);
                if (result.Length > 0)
                {
                    bool added = false;
                    for (int j = 0; j < result.Length; j++)
                    {
                        if (newNode.Total() < result[j].Total())
                        {
                            result.Add(result[^1]);

                            added = true;

                            for (int x = result.Length - 2; x > j; x--)
                                result[x] = result[x - 1];

                            result[j] = newNode;

                            break;
                        }
                    }

                    if (!added)
                        result.Add(newNode);
                }
                else
                    result.Add(newNode);

                for (int t = 0; t < result.Length; t++)
                {
                    if (!this.ContainsID(triangles[result[i].triangleID].neighbors, toAddTriangleID[i]))
                        continue;

                    if (result[t].previousCost > newNode.cost)
                        continue;

                    result[t] = new Node(newNode, result[t], areas, this.simpleVerts, triangles);
                }
            }

            toCheck.Dispose();

            return result;
        }

        private readonly bool ContainsID(UnsafeList<Node> list, int target)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].triangleID == target)
                    return true;

            return false;
        }

        private readonly bool ContainsID(UnsafeList<int> list, int target)
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
        public readonly UnsafeList<int> nodePath;
        private readonly UnsafeList<Node> checkList;

        public JobPath(Node lastNode, UnsafeList<Node> checkedNodes, NativeArray<JobTriangle> triangles)
        {
            Node checking = lastNode;
            this.checkList = new UnsafeList<Node>(checkedNodes.Length, Allocator.Temp);
            int breaker = 0;
            while (checking.triangleID != -1)
            {
                breaker++;
                if (breaker == checkedNodes.Length)
                    break;

                this.checkList.Add(checking);

                if (checking.previousNodeTriangleID == -1)
                    break;

                Node bestNeighbor = Get(checking.previousNodeTriangleID, checkedNodes);
                for (int i = 0; i < checkedNodes.Length; i++)
                {
                    if (Contains(triangles[checking.triangleID].neighbors, checkedNodes[i].triangleID) &&
                        checkedNodes[i].cost < bestNeighbor.cost)
                        bestNeighbor = checkedNodes[i];
                }

                checking = bestNeighbor;
            }

            this.nodePath = new UnsafeList<int>(this.checkList.Length, Allocator.TempJob);
            for (int i = 0; i < this.checkList.Length; i++)
                this.nodePath.Add(this.checkList[i].triangleID);

            this.checkList.Dispose();
        }

        public void Dispose()
        {
            if (this.nodePath.IsCreated)
                this.nodePath.Dispose();
            if (this.checkList.IsCreated)
                this.checkList.Dispose();
        }

        private static Node Get(int previousID, UnsafeList<Node> nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i].triangleID == previousID)
                    return nodes[i];

            return new Node();
        }

        private static bool Contains(UnsafeList<int> nodes, int target)
        {
            foreach (int t in nodes)
                if (t == target)
                    return true;

            return false;
        }
    }

    public readonly struct JobAgent
    {
        #region Values

        public readonly int currentTriangleID, destinationTriangleID;

        public readonly float3 endPoint, startPosition;

        public readonly float radius;

        #endregion

        #region Build In States

        public JobAgent(QueuedAgentRequest request)
        {
            UnitAgent agent = request.agent;
            UnitAgentSettings settings = agent.Settings;

            this.currentTriangleID = agent.CurrentTriangleIndex();
            this.endPoint = request.destination;
            this.startPosition = agent.transform.position;
            this.destinationTriangleID = UnitNavigation.ClosestTriangleIndex(this.endPoint);
            this.radius = settings.Radius;
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
            foreach (int t in triangle.Neighbors)
                this.neighbors.Add(t);

            this.navPoints = new UnsafeList<int>(triangle.NavPoints.Count, Allocator.Temp);
            foreach (int t in triangle.NavPoints)
                this.navPoints.Add(t);

            this.widths = new UnsafeList<float>(triangle.Widths.Count, Allocator.Temp);
            foreach (float t in triangle.Widths)
                this.widths.Add(t);
        }

        public void Dispose()
        {
            if (this.neighbors.IsCreated)
                this.neighbors.Dispose();
            if (this.navPoints.IsCreated)
                this.navPoints.Dispose();
            if (this.widths.IsCreated)
                this.widths.Dispose();
        }

        #endregion
    }

    public readonly struct Node
    {
        public readonly int triangleID;
        public readonly float cost, dist, previousCost;
        public readonly int previousNodeTriangleID;

        public Node(Node newPrevious, Node oldNode, NativeArray<int> areas, NativeArray<float2> simpleVerts,
            NativeArray<JobTriangle> triangles)
        {
            this.triangleID = oldNode.triangleID;
            this.dist = oldNode.dist;

            this.previousNodeTriangleID = newPrevious.previousNodeTriangleID;
            this.previousCost = newPrevious.cost;
            this.cost = newPrevious.cost + (areas[this.triangleID] + 1) * math.distance(
                Center(triangles[this.triangleID], simpleVerts),
                Center(triangles[newPrevious.triangleID], simpleVerts));
        }

        public Node(JobTriangle triangle, NativeArray<float2> simpleVerts, float2 destination, Node previousNode,
            NativeArray<int> areas, NativeArray<JobTriangle> triangles)
        {
            this.previousNodeTriangleID = previousNode.triangleID;
            this.triangleID = triangle.id;
            this.previousCost = previousNode.cost;
            this.cost = this.previousCost + (areas[this.triangleID] + 1) * math.distance(Center(triangle, simpleVerts),
                Center(triangles[previousNode.triangleID], simpleVerts));
            this.dist = math.distance(Center(triangle, simpleVerts), destination);
        }

        public Node(JobTriangle triangle, NativeArray<float2> simpleVerts, float2 destination, float startCost)
        {
            this.previousNodeTriangleID = -1;
            this.triangleID = triangle.id;
            this.cost = startCost;
            this.previousCost = 0;
            this.dist = math.distance(Center(triangle, simpleVerts),
                destination);
        }

        public float Total() => this.cost + this.dist;

        private static float2 Center(JobTriangle triangle, NativeArray<float2> simpleVerts) =>
            math.lerp(math.lerp(simpleVerts[triangle.a], simpleVerts[triangle.b], .5f), simpleVerts[triangle.c], .5f);
    }
}