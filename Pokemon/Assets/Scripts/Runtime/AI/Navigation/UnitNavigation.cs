#region Libraries

using Runtime.Common;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.AI.Navigation
{
    #region Enums

    internal enum TriangleLine
    {
        AB,
        BC,
        AC
    }

    #endregion

    public static class UnitNavigation
    {
        #region Values

        private static CalculatedNavMesh NavMesh = null;

        private static UnityAction OnNavMeshChanged;

        public static bool Ready => NavMesh != null;

        private static List<QueuedAgentRequest> requests;

        private static NativeArray<float3> verts;
        private static NativeArray<float2> simpleVerts;
        private static NativeArray<int> areas;
        private static NativeArray<JobTriangle> triangles;

        private static bool switchNativeArrays;

        private static int maxCalculationsPerBatch = 100;

        private static JobHandle currentJob;

        #endregion

        #region Getters

        public static Vector3[] GetVerts() => NavMesh.Vertices();

        #endregion

        #region Setters

        public static bool SetNavMesh(CalculatedNavMesh set)
        {
            if (NavMesh == set)
                return false;

            //Debug.Log($"{NavMesh?.name}  -  {set?.name}");

            NavMesh = set;

            OnNavMeshChanged?.Invoke();

            switchNativeArrays = true;

            return true;
        }

        #endregion

        #region In

        public static void AddOnNavMeshChange(UnityAction action)
            => OnNavMeshChanged += action;

        public static void RemoveOnNavMeshChange(UnityAction action)
            => OnNavMeshChanged -= action;

        public static void QueueForPath(UnitAgent agent, Vector3 destination) =>
            requests.Add(new QueuedAgentRequest(destination, agent));

        public static int PlaceAgentOnNavMesh(UnitAgent agent)
        {
            Vector2 p = agent.transform.position.XZ();
            for (int i = 0; i < NavMesh.Triangles.Length; i++)
            {
                int[] ids = NavMesh.Triangles[i].Vertices;
                if (ExtMathf.PointWithinTriangle2D(p,
                    NavMesh.SimpleVertices[ids[0]],
                    NavMesh.SimpleVertices[ids[1]],
                    NavMesh.SimpleVertices[ids[2]]))
                {
                    agent.transform.position = new Vector3(p.x, NavMesh.Triangles[i].MaxY, p.y);
                    return i;
                }
            }

            NavTriangle navTriangle = NavMesh.Triangles.RandomFrom();
            agent.transform.position = navTriangle.Center(NavMesh.Vertices());
            return navTriangle.ID;
        }

        #endregion

        #region Out

        public static int ClosestTriangleIndex(Vector3 p) => NavMesh.ClosestTriangleIndex(p);

        public static NavTriangle GetTriangleByID(int id) => NavMesh.Triangles[id];

        public static Vector3 Get3DVertByIndex(int id) => NavMesh.Vertices()[id];

        public static Vector3 Get2DVertByIndex(int id) => NavMesh.SimpleVertices[id];

        public static Vector3[] Get3DVertByIndex(params int[] id) => id.Select(i => NavMesh.Vertices()[i]).ToArray();

        public static Vector2[] Get2DVertByIndex(params int[] id) => id.Select(i => NavMesh.SimpleVertices[i]).ToArray();

        #endregion

        #region Internal

        [RuntimeInitializeOnLoadMethod()]
        private static void Initialize()
        {
            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.subSystemList[5].updateDelegate += Update;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);

            requests = new List<QueuedAgentRequest>();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
        }

        private static void Update()
        {
            UpdateNativeArrays();
            CalculatePaths();
        }

        private static void UpdateNativeArrays()
        {
            if (!switchNativeArrays)
                return;

            DisposeNatives();

            Vector3[] navVerts = NavMesh.Vertices();
            verts = new NativeArray<float3>(navVerts.Length, Allocator.Persistent);
            for (int i = 0; i < navVerts.Length; i++)
                verts[i] = navVerts[i];

            Vector2[] navSimpleVerts = NavMesh.SimpleVertices;
            simpleVerts = new NativeArray<float2>(navSimpleVerts.Length, Allocator.Persistent);
            for (int i = 0; i < navSimpleVerts.Length; i++)
                simpleVerts[i] = navSimpleVerts[i];

            int[] navAreas = NavMesh.Areas;
            areas = new NativeArray<int>(navAreas, Allocator.Persistent);

            NavTriangle[] navTriangles = NavMesh.Triangles.ToArray();
            triangles = new NativeArray<JobTriangle>(navTriangles.Length, Allocator.Persistent);
            for (int i = 0; i < navTriangles.Length; i++)
                triangles[i] = new JobTriangle(navTriangles[i]);

            switchNativeArrays = false;
        }

        private static void CalculatePaths()
        {
            if (requests.Count == 0)
                return;

            int count = requests.Count < maxCalculationsPerBatch ? requests.Count : maxCalculationsPerBatch;
            NativeArray<JobAgent> agents = new(count, Allocator.TempJob);
            NativeArray<JobPath> paths = new(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
                agents[i] = new JobAgent(requests[i]);

            PathCalculatorJob calculationJob = new(paths, agents, verts, simpleVerts, areas, triangles);

            currentJob = calculationJob.Schedule(requests.Count, 100);
            currentJob.Complete();

            for (int i = 0; i < requests.Count; i++)
                requests[i].agent.SetPath(CastToUnitPath(agents[i], paths[i]));

            requests.Clear();
            agents.Dispose();
            paths.Dispose();
        }

        private static void DisposeNatives()
        {
            if (verts.IsCreated)
                verts.Dispose();

            if (simpleVerts.IsCreated)
                simpleVerts.Dispose();

            if (areas.IsCreated)
                areas.Dispose();

            if (triangles.IsCreated)
                triangles.Dispose();
        }

        private static UnitPath CastToUnitPath(JobAgent jobAgent, JobPath jobPath)
        {
            int[] ids = new int[jobPath.nodePath.Length];
            for (int i = 0; i < jobPath.nodePath.Length; i++)
                ids[i] = jobPath.nodePath[i];

            return new(new Vector3(jobAgent.desination.x, jobAgent.desination.y, jobAgent.desination.z),
                ids,
                NavMesh.Triangles,
                NavMesh.Vertices(),
                jobAgent.startPosition,
                jobAgent.radius);
        }

#if UNITY_EDITOR
        private static void OnExitPlayMode(PlayModeStateChange state)
        {
            if (!state.Equals(PlayModeStateChange.ExitingPlayMode))
                return;

            if (!currentJob.IsCompleted)
                currentJob.Complete();
            DisposeNatives();
            NavMesh = null;

            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.subSystemList[5].updateDelegate -= Update;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
        }
#endif

        #endregion
    }

    #region Job

    public readonly struct QueuedAgentRequest
    {
        #region Values

        public readonly Vector3 destination;

        public readonly UnitAgent agent;

        #endregion

        #region Build In States

        public QueuedAgentRequest(Vector3 destination, UnitAgent agent)
        {
            this.destination = destination;
            this.agent = agent;
        }

        #endregion
    }

    #endregion
}