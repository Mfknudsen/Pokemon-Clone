#region Libraries

using System.Collections.Generic;
using System.Linq;
using Runtime.Core;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.AI.Navigation
{
    public static class UnitNavigation
    {
        #region Values

        private static CalculatedNavMesh _navMesh;

        private static UnityAction _onNavMeshChanged;

        public static bool Ready => _navMesh != null;

        private static List<QueuedAgentRequest> _requests;

        private static NativeArray<float3> _verts;
        private static NativeArray<float2> _simpleVerts;
        private static NativeArray<int> _areas;
        private static NativeArray<JobTriangle> _triangles;

        private static bool _switchNativeArrays;

        private const int MAX_CALCULATIONS_PER_BATCH = 100;

        private static JobHandle _currentJob;

        #endregion

        #region Getters

        public static Vector3[] GetVerts() => _navMesh.Vertices();

        #endregion

        #region Setters

        public static void SetNavMesh(CalculatedNavMesh set)
        {
            if (_navMesh == set)
                return;

            _navMesh = set;

            _onNavMeshChanged?.Invoke();

            _switchNativeArrays = true;
        }

        #endregion

        #region In

        public static void AddOnNavMeshChange(UnityAction action)
            => _onNavMeshChanged += action;

        public static void RemoveOnNavMeshChange(UnityAction action)
            => _onNavMeshChanged -= action;

        public static void QueueForPath(UnitAgent agent, Vector3 destination) =>
            _requests.Add(new QueuedAgentRequest(destination, agent));

        public static int PlaceAgentOnNavMesh(UnitAgent agent)
        {
            Vector3 agentPosition = agent.transform.position;
            Vector3[] vertices = _navMesh.Vertices();
            Vector2 agentXZ = agentPosition.XZ();

            foreach (NavTriangle navTriangle in _navMesh.Triangles)
            {
                if (!MathC.PointWithinTriangle2D(agentXZ,
                        _navMesh.SimpleVertices[navTriangle.GetA],
                        _navMesh.SimpleVertices[navTriangle.GetB],
                        _navMesh.SimpleVertices[navTriangle.GetC],
                        0))
                    continue;

                agent.transform.position = new Vector3(agentPosition.x, navTriangle.MaxY, agentPosition.z);
                return navTriangle.ID;
            }

            float dist = (vertices[0] - agentPosition).sqrMagnitude;
            int selected = 0;

            for (int i = 1; i < vertices.Length; i++)
            {
                float newDist = (vertices[i] - agentPosition).sqrMagnitude;
                if (newDist > dist)
                    continue;

                dist = newDist;
                selected = i;
            }

            List<int> trianglesByVertexID = _navMesh.GetTrianglesByVertexID(selected);
            foreach (int navTriangleID in trianglesByVertexID)
            {
                NavTriangle navTriangle = _navMesh.Triangles[navTriangleID];
                if (!MathC.PointWithinTriangle2D(agentXZ,
                        _navMesh.SimpleVertices[navTriangle.GetA],
                        _navMesh.SimpleVertices[navTriangle.GetB],
                        _navMesh.SimpleVertices[navTriangle.GetC],
                        0))
                    continue;

                agent.transform.position = new Vector3(agentPosition.x, navTriangle.MaxY, agentPosition.z);
                return navTriangle.ID;
            }

            agent.transform.position = vertices[selected];
            return trianglesByVertexID.RandomFrom();
        }

        #endregion

        #region Out

        public static int ClosestTriangleIndex(Vector3 p) => _navMesh.ClosestTriangleIndex(p);

        public static NavTriangle GetTriangleByID(int id) => _navMesh.Triangles[id];

        public static Vector3 Get3DVertByIndex(int id) => _navMesh.Vertices()[id];

        public static Vector3 Get2DVertByIndex(int id) => _navMesh.SimpleVertices[id];

        public static Vector3[] Get3DVertByIndex(params int[] id) => id.Select(i => _navMesh.Vertices()[i]).ToArray();

        public static Vector2[] Get2DVertByIndex(params int[] id) =>
            id.Select(i => _navMesh.SimpleVertices[i]).ToArray();

        #endregion

        #region Internal

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.subSystemList[5].updateDelegate += Update;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);

            _requests = new List<QueuedAgentRequest>();

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
            if (!_switchNativeArrays)
                return;

            DisposeNatives();

            Vector3[] navVerts = _navMesh.Vertices();
            _verts = new NativeArray<float3>(navVerts.Length, Allocator.Persistent);
            for (int i = 0; i < navVerts.Length; i++)
                _verts[i] = navVerts[i];

            Vector2[] navSimpleVerts = _navMesh.SimpleVertices;
            _simpleVerts = new NativeArray<float2>(navSimpleVerts.Length, Allocator.Persistent);
            for (int i = 0; i < navSimpleVerts.Length; i++)
                _simpleVerts[i] = navSimpleVerts[i];

            int[] navAreas = _navMesh.Areas;
            _areas = new NativeArray<int>(navAreas, Allocator.Persistent);

            NavTriangle[] navTriangles = _navMesh.Triangles.ToArray();
            _triangles = new NativeArray<JobTriangle>(navTriangles.Length, Allocator.Persistent);
            for (int i = 0; i < navTriangles.Length; i++)
                _triangles[i] = new JobTriangle(navTriangles[i]);

            _switchNativeArrays = false;
        }

        private static void CalculatePaths()
        {
            if (_requests.Count == 0)
                return;

            int count = _requests.Count < MAX_CALCULATIONS_PER_BATCH ? _requests.Count : MAX_CALCULATIONS_PER_BATCH;
            NativeArray<JobAgent> agents = new NativeArray<JobAgent>(count, Allocator.TempJob);
            NativeArray<JobPath> paths = new NativeArray<JobPath>(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
                agents[i] = new JobAgent(_requests[i]);

            PathCalculatorJob calculationJob =
                new PathCalculatorJob(paths, agents, _verts, _simpleVerts, _areas, _triangles);

            _currentJob = calculationJob.Schedule(_requests.Count, 100);
            _currentJob.Complete();

            for (int i = 0; i < _requests.Count; i++)
                _requests[i].agent.SetPath(CastToUnitPath(agents[i], paths[i], _requests[i].agent));

            _requests.Clear();
            agents.Dispose();
            paths.Dispose();
        }

        private static void DisposeNatives()
        {
            if (_verts.IsCreated)
                _verts.Dispose();

            if (_simpleVerts.IsCreated)
                _simpleVerts.Dispose();

            if (_areas.IsCreated)
                _areas.Dispose();

            if (_triangles.IsCreated)
                _triangles.Dispose();
        }

        private static UnitPath CastToUnitPath(JobAgent jobAgent, JobPath jobPath, UnitAgent agent)
        {
            int[] ids = new int[jobPath.nodePath.Length];
            for (int i = 0; i < jobPath.nodePath.Length; i++)
                ids[i] = jobPath.nodePath[jobPath.nodePath.Length - 1 - i];

            return new UnitPath(
                jobAgent.startPosition,
                jobAgent.endPoint,
                ids,
                _navMesh.Triangles,
                _navMesh.Vertices(),
                _navMesh.SimpleVertices,
                agent);
        }

#if UNITY_EDITOR
        private static void OnExitPlayMode(PlayModeStateChange state)
        {
            if (!state.Equals(PlayModeStateChange.ExitingPlayMode))
                return;

            if (!_currentJob.IsCompleted)
                _currentJob.Complete();
            DisposeNatives();
            _navMesh = null;

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