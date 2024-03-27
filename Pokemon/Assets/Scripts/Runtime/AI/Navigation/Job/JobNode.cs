#region Libraries

using Unity.Collections;
using Unity.Mathematics;

#endregion

namespace Runtime.AI.Navigation.Job
{
    public readonly struct JobNode
    {
        public readonly int triangleID;
        public readonly float cost, previousCost;
        private readonly float dist;
        public readonly int previousNodeTriangleID;

        public JobNode(JobNode newPrevious, JobNode oldJobNode, NativeArray<int> areas, NativeArray<float2> simpleVerts,
            NativeArray<JobTriangle> triangles)
        {
            this.triangleID = oldJobNode.triangleID;
            this.dist = oldJobNode.dist;

            this.previousNodeTriangleID = newPrevious.previousNodeTriangleID;
            this.previousCost = newPrevious.cost;
            this.cost = newPrevious.cost + (areas[this.triangleID] + 1) * math.distance(
                Center(triangles[this.triangleID], simpleVerts),
                Center(triangles[newPrevious.triangleID], simpleVerts));
        }

        public JobNode(JobTriangle triangle, NativeArray<float2> simpleVerts, float2 destination,
            JobNode previousJobNode,
            NativeArray<int> areas, NativeArray<JobTriangle> triangles)
        {
            this.previousNodeTriangleID = previousJobNode.triangleID;
            this.triangleID = triangle.id;
            this.previousCost = previousJobNode.cost;
            this.cost = this.previousCost + (areas[this.triangleID] + 1) * math.distance(Center(triangle, simpleVerts),
                Center(triangles[previousJobNode.triangleID], simpleVerts));
            this.dist = math.distance(Center(triangle, simpleVerts), destination);
        }

        public JobNode(JobTriangle triangle, NativeArray<float2> simpleVerts, float2 destination, float startCost)
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