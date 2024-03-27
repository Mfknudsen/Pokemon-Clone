#region Libraries

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI.Navigation.Job;
using Runtime.Core;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

#endregion

namespace Runtime.AI.Navigation
{
    public static class UnitNavigation
    {
        #region Values

        /// <summary>
        ///     The current main navigation mesh
        /// </summary>
        private static NavigationMesh _navMesh;

        /// <summary>
        ///     For when navigation mesh changes then all agents will try to recalculate their path to use the new mesh
        /// </summary>
        private static UnityAction _onNavMeshChanged;

        /// <summary>
        ///     All currently active agents
        /// </summary>
        private static List<UnitAgent> _allUnitAgents;

        /// <summary>
        ///     Agents split into groups based on their x and z coordinate
        /// </summary>
        private static List<UnitAgent>[,] _groupedUnitAgents;

        private static List<int>[,] _groupedTriangleIds;

        private static List<QueuedAgentRequest> _requests;

        private static NativeArray<float3> _verts;
        private static NativeArray<float2> _simpleVerts;
        private static NativeArray<int> _areas;
        private static NativeArray<JobTriangle> _triangles;

        private static bool _switchNativeArrays;

        /// <summary>
        ///     How many path jobs can be calculated per batch
        /// </summary>
        private const int MAX_CALCULATIONS_PER_BATCH = 1000;

        private static JobHandle _currentJob;

        private static LayerMask _unitAgentLayer;

        #endregion

        #region Getters

        /// <summary>
        ///     Navigation should only be used when there is a navigation mesh to use
        /// </summary>
        public static bool Ready => _navMesh != null;

        public static Vector3[] GetVerts() => _navMesh.Vertices();

        public static LayerMask GetUnitAgentLayerMask() => _unitAgentLayer;

        #endregion

        #region Setters

        public static void SetNavMesh(NavigationMesh set)
        {
            if (_navMesh == set)
                return;

            _navMesh = set;

            ResetAgentGrouping();
            ResetTriangleGrouping();

            _onNavMeshChanged?.Invoke();

            _switchNativeArrays = true;
        }

        #endregion

        #region In

        internal static void AddAgent(UnitAgent agent)
        {
            if (_allUnitAgents.Contains(agent)) return;

            _allUnitAgents.Add(agent);

            if (_navMesh == null) return;

            Vector2Int id = GroupingIDByPosition(agent.transform.position);
            _groupedUnitAgents[id.x, id.y].Add(agent);
        }

        internal static void RemoveAgent(UnitAgent agent)
            => _allUnitAgents.Remove(agent);

        public static void AddOnNavMeshChange(UnityAction action)
            => _onNavMeshChanged += action;

        public static void RemoveOnNavMeshChange(UnityAction action)
            => _onNavMeshChanged -= action;

        public static void QueueForPath(UnitAgent agent, Vector3 destination)
        {
            _requests.Add(new QueuedAgentRequest(destination, agent));
        }

        public static int PlaceAgentOnNavMesh(UnitAgent agent)
        {
            Vector3 agentPosition = agent.transform.position;
            Vector2 agentPosition2D = agentPosition.XZ();

            List<int> triangleIds = GetTriangleIdsByPosition(agentPosition);

            if (triangleIds.Count == 0)
                triangleIds = GetTriangleIdsByPositionSpiralOutwards(agentPosition, 1, 2, 1);

            foreach (int i in triangleIds)
            {
                NavTriangle t = _navMesh.Triangles[i];

                if (!MathC.PointWithinTriangle2D(agentPosition2D,
                        _navMesh.SimpleVertices[t.GetA],
                        _navMesh.SimpleVertices[t.GetB],
                        _navMesh.SimpleVertices[t.GetC],
                        out float w1,
                        out float w2))
                    continue;

                Vector3 vectorA = _navMesh.VertByIndex(t.GetA),
                    vectorB = _navMesh.VertByIndex(t.GetB),
                    vectorC = _navMesh.VertByIndex(t.GetC);

                agent.Place(vectorA + (vectorB - vectorA) * w1 + (vectorC - vectorA) * w2);

                return i;
            }

            foreach (int triangleId in triangleIds)
            {
                NavTriangle t = _navMesh.Triangles[triangleId];
                MathC.PointWithinTriangle2D(agentPosition2D,
                    _navMesh.SimpleVertices[t.GetA],
                    _navMesh.SimpleVertices[t.GetB],
                    _navMesh.SimpleVertices[t.GetC],
                    out float w1,
                    out float w2);

                Debug.Log($"{w1}   |   {w2}   |   {w1 + w2}");
            }

            int result = triangleIds[0];

            int firstVertexIndex = _navMesh.Triangles[triangleIds[0]].GetA;

            float firstDist = agentPosition.QuickSquareDistance(_navMesh.VertByIndex(firstVertexIndex));

            foreach (int id in triangleIds)
            foreach (int vertex in _navMesh.Triangles[id].Vertices)
            {
                if (firstVertexIndex == vertex)
                    continue;

                float dist = agentPosition.QuickSquareDistance(_navMesh.VertByIndex(vertex));

                if (dist > firstDist) continue;

                firstDist = dist;
                firstVertexIndex = vertex;
                result = vertex;
            }

            int secondVertexIndex = _navMesh.Triangles[result].GetA;
            if (firstVertexIndex == secondVertexIndex)
                secondVertexIndex = _navMesh.Triangles[result].GetB;

            foreach (int vertex in _navMesh.Triangles[result].Vertices)
            {
                if (vertex == firstVertexIndex || vertex == secondVertexIndex)
                    continue;

                if (agentPosition.QuickSquareDistance(_navMesh.VertByIndex(vertex)) >
                    agentPosition.QuickSquareDistance(_navMesh.VertByIndex(secondVertexIndex)))
                    continue;

                secondVertexIndex = vertex;
                break;
            }

            //Place
            Vector3 center = _navMesh.Triangles[result].Center(_navMesh);

            Vector3 a = _navMesh.VertByIndex(firstVertexIndex),
                b = _navMesh.VertByIndex(secondVertexIndex);

            a += (center - a).normalized * agent.Settings.Radius;
            b += (center - b).normalized * agent.Settings.Radius;

            agent.Place(MathC.ClosetPointOnLine(agentPosition, a, b));

            return result;
        }

        #endregion

        #region Out

        public static int ClosestTriangleIndex(Vector3 p) => _navMesh.ClosestTriangleIndex(p);

        public static NavTriangle GetTriangleByID(int id) => _navMesh.Triangles[id];

        public static Vector3 Get3DVertByIndex(int id) => _navMesh.Vertices()[id];

        public static Vector2 Get2DVertByIndex(int id) => _navMesh.SimpleVertices[id];

        public static Vector3[] Get3DVertByIndex(params int[] id) => id.Select(i => _navMesh.Vertices()[i]).ToArray();

        public static Vector2[] Get2DVertByIndex(params int[] id) =>
            id.Select(i => _navMesh.SimpleVertices[i]).ToArray();

        /// <summary>
        ///     Get a list of agents 3x3 radius based on the grouped 2D list of agents
        /// </summary>
        /// <param name="agent">Will get agents around this agent while also not including it in the result</param>
        /// <returns>Agents around input agent</returns>
        public static List<UnitAgent> GetAgentsByAgentPosition(UnitAgent agent)
        {
            Vector2Int id = GroupingIDByPosition(agent.transform.position);

            List<UnitAgent> result = new List<UnitAgent>();

            for (int x = -1; x <= 1; x++)
            {
                if (id.x + x < 0 ||
                    id.x + x >= _groupedUnitAgents.GetLength(0))
                    continue;

                for (int y = -1; y <= 1; y++)
                {
                    if (id.y + y < 0 ||
                        id.y + y >= _groupedUnitAgents.GetLength(1))
                        continue;

                    foreach (UnitAgent toAdd in _groupedUnitAgents[id.x + x, id.y + y])
                    {
                        if (Mathf.Abs(toAdd.transform.position.y - agent.transform.position.y) > 50)
                            continue;

                        result.Add(toAdd);
                    }
                }
            }

            result.Remove(agent);

            return result;
        }

        public static List<int> GetTriangleIdsByPosition(Vector3 pos)
        {
            Vector2Int id = GroupingIDByPosition(pos);

            List<int> result = new List<int>();

            for (int x = -1; x <= 1; x++)
            {
                if (id.x + x < 0 || id.x + x >= _groupedTriangleIds.GetLength(0))
                    continue;

                for (int y = -1; y <= 1; y++)
                {
                    if (id.y + y < 0 || id.y + y >= _groupedTriangleIds.GetLength(1))
                        continue;

                    foreach (int i in _groupedTriangleIds[id.x + x, id.y + y])
                        if (!result.Contains(i))
                            result.Add(i);
                }
            }

            return result;
        }

        public static List<int> GetTriangleIdsByPositionSpiralOutwards(Vector3 pos, int min, int max,
            int increaseForSpiral)
        {
            if (max <= min) throw new Exception("Max must be greater then min");

            if (increaseForSpiral < 0) throw new Exception("Increase must be zero or greater");

            List<int> result = new List<int>();
            while (result.Count == 0 &&
                   (max - (max - min) < _groupedTriangleIds.GetLength(0) ||
                    max - (max - min) < _groupedTriangleIds.GetLength(1)))
            {
                Vector2Int id = GroupingIDByPosition(pos);

                for (int x = -max; x <= max; x++)
                {
                    if (Mathf.Abs(x) <= min) continue;

                    if (id.x + x < 0 || id.x + x >= _groupedTriangleIds.GetLength(0)) continue;

                    for (int y = -max; y <= max; y++)
                    {
                        if (id.y + y < 0 || id.y + y >= _groupedTriangleIds.GetLength(1)) continue;

                        foreach (int i in _groupedTriangleIds[id.x + x, id.y + y])
                            if (!result.Contains(i))
                                result.Add(i);
                    }
                }

                if (increaseForSpiral == 0)
                    break;

                min += increaseForSpiral;
                max += increaseForSpiral;
            }

            return result;
        }

        #endregion

        #region Internal

        /// <summary>
        ///     Add the update function to the game loop and setup the request list.
        ///     In editor it will need to add the on exit play mode function to stop the update from happening multiple times
        ///     during play mode an while play mode is not active.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type == typeof(FixedUpdate))
                    playerLoop.subSystemList[i].updateDelegate += UpdateAgents;

                if (playerLoop.subSystemList[i].type == typeof(PostLateUpdate))
                    playerLoop.subSystemList[i].updateDelegate += UpdateNavigationValues;
            }

            PlayerLoop.SetPlayerLoop(playerLoop);

            _requests = new List<QueuedAgentRequest>();
            _allUnitAgents = new List<UnitAgent>();

            _unitAgentLayer = LayerMask.NameToLayer("AI");

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Clean up on exiting play mode.
        /// </summary>
        /// <param name="state">State giving by Unity</param>
        private static void OnExitPlayMode(PlayModeStateChange state)
        {
            if (!state.Equals(PlayModeStateChange.ExitingPlayMode))
                return;

            if (!_currentJob.IsCompleted)
                _currentJob.Complete();
            DisposeNatives();
            _navMesh = null;

            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type == typeof(FixedUpdate))
                    playerLoop.subSystemList[i].updateDelegate -= UpdateAgents;

                if (playerLoop.subSystemList[i].type == typeof(PostLateUpdate))
                    playerLoop.subSystemList[i].updateDelegate -= UpdateNavigationValues;
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
        }
#endif

        /// <summary>
        ///     Update all currently active agents.
        ///     Using one update call instead of each agent having their own individual call reduces time spent by Unity.
        /// </summary>
        private static void UpdateAgents()
        {
            foreach (UnitAgent unitAgent in _allUnitAgents)
                unitAgent.UpdateAgent();
        }

        /// <summary>
        ///     Update the navigation loop
        /// </summary>
        private static void UpdateNavigationValues()
        {
            UpdateNativeArrays();
            CalculatePaths();
        }

        /// <summary>
        ///     When a new navmesh is set ass current its values will be added to native array for use during the pathing jobs
        /// </summary>
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

        /// <summary>
        ///     Resets the unit agent grouping based on the current navigation mesh
        /// </summary>
        private static void ResetAgentGrouping()
        {
            int lengthX =
                    Mathf.FloorToInt((_navMesh.GetMaxX() - _navMesh.GetMinX()) / _navMesh.GetGroupDivisionSize()),
                lengthY =
                    Mathf.FloorToInt((_navMesh.GetMaxZ() - _navMesh.GetMinZ()) / _navMesh.GetGroupDivisionSize());

            _groupedUnitAgents = new List<UnitAgent>[lengthX, lengthY];

            for (int x = 0; x < _groupedUnitAgents.GetLength(0); x++)
            for (int y = 0; y < _groupedUnitAgents.GetLength(1); y++)
                _groupedUnitAgents[x, y] = new List<UnitAgent>();

            foreach (UnitAgent agent in _allUnitAgents)
            {
                Vector2Int id = GroupingIDByPosition(agent.transform.position);
                _groupedUnitAgents[id.x, id.y].Add(agent);
            }
        }

        private static void ResetTriangleGrouping()
        {
            int lengthX =
                    Mathf.FloorToInt((_navMesh.GetMaxX() - _navMesh.GetMinX()) / _navMesh.GetGroupDivisionSize()),
                lengthY =
                    Mathf.FloorToInt((_navMesh.GetMaxZ() - _navMesh.GetMinZ()) / _navMesh.GetGroupDivisionSize());

            _groupedTriangleIds = new List<int>[lengthX, lengthY];

            Vector2 startPoint = new Vector2(_navMesh.GetMinX(), _navMesh.GetMinZ());

            for (int x = 0; x < _groupedTriangleIds.GetLength(0); x++)
            for (int y = 0; y < _groupedTriangleIds.GetLength(1); y++)
            {
                _groupedTriangleIds[x, y] = new List<int>();

                foreach (NavTriangle t in _navMesh.Triangles)
                    if (MathC.PointWithinTriangle2D(
                            startPoint + new Vector2(
                                x * _navMesh.GetGroupDivisionSize(),
                                y * _navMesh.GetGroupDivisionSize()),
                            _navMesh.SimpleVertices[t.GetA],
                            _navMesh.SimpleVertices[t.GetB],
                            _navMesh.SimpleVertices[t.GetC])
                        ||
                        MathC.PointWithinTriangle2D(
                            startPoint + new Vector2(
                                (x + 1) * _navMesh.GetGroupDivisionSize(),
                                y * _navMesh.GetGroupDivisionSize()),
                            _navMesh.SimpleVertices[t.GetA],
                            _navMesh.SimpleVertices[t.GetB],
                            _navMesh.SimpleVertices[t.GetC])
                        ||
                        MathC.PointWithinTriangle2D(
                            startPoint + new Vector2(
                                x * _navMesh.GetGroupDivisionSize(),
                                (y + 1) * _navMesh.GetGroupDivisionSize()),
                            _navMesh.SimpleVertices[t.GetA],
                            _navMesh.SimpleVertices[t.GetB],
                            _navMesh.SimpleVertices[t.GetC])
                        ||
                        MathC.PointWithinTriangle2D(
                            startPoint + new Vector2(
                                (x + 1) * _navMesh.GetGroupDivisionSize(),
                                (y + 1) * _navMesh.GetGroupDivisionSize()),
                            _navMesh.SimpleVertices[t.GetA],
                            _navMesh.SimpleVertices[t.GetB],
                            _navMesh.SimpleVertices[t.GetC]))
                    {
                        _groupedTriangleIds[x, y].Add(t.ID);
                    }
                    else
                    {
                        float xMin = startPoint.x + x * _navMesh.GetGroupDivisionSize(),
                            xMax = xMin + (x + 1) * _navMesh.GetGroupDivisionSize(),
                            zMin = startPoint.y + y * _navMesh.GetGroupDivisionSize(),
                            zMax = zMin + (y + 1) * _navMesh.GetGroupDivisionSize();

                        foreach (int tVertex in t.Vertices)
                        {
                            if (_navMesh.SimpleVertices[tVertex].x < xMin ||
                                _navMesh.SimpleVertices[tVertex].x > xMax ||
                                _navMesh.SimpleVertices[tVertex].y < zMin ||
                                _navMesh.SimpleVertices[tVertex].y > zMax)
                                continue;

                            _groupedTriangleIds[x, y].Add(t.ID);
                            break;
                        }
                    }
            }
        }

        /// <summary>
        ///     Calculate paths using jobs
        /// </summary>
        private static void CalculatePaths()
        {
            if (_requests.Count == 0)
                return;

            int count = _requests.Count < MAX_CALCULATIONS_PER_BATCH ? _requests.Count : MAX_CALCULATIONS_PER_BATCH;
            NativeArray<JobAgent> agents = new NativeArray<JobAgent>(count, Allocator.TempJob);
            NativeArray<JobPath> paths = new NativeArray<JobPath>(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
                agents[i] = new JobAgent(_requests[i]);

            AStartCalculationJob job =
                new AStartCalculationJob(paths, agents, _simpleVerts, _areas, _triangles);

            _currentJob = job.Schedule(_requests.Count, 100);
            _currentJob.Complete();

            for (int i = 0; i < _requests.Count; i++)
            {
                _requests[i].agent.SetPath(ToUnitPath(agents[i], paths[i], _requests[i].agent));
                paths[i].Dispose();
            }

            if (agents.IsCreated)
                agents.Dispose();
            if (paths.IsCreated)
                paths.Dispose();

            _requests.Clear();
        }

        /// <summary>
        ///     Dispose native arrays to insure no memory leaks
        /// </summary>
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

        private static UnitPath ToUnitPath(JobAgent jobAgent, JobPath jobPath, UnitAgent agent)
        {
            int[] ids = new int[jobPath.nodePath.Length];
            for (int i = 0; i < jobPath.nodePath.Length; i++)
                ids[i] = jobPath.nodePath[jobPath.nodePath.Length - 1 - i];

            return new UnitPath();

            return new UnitPath(
                jobAgent.startPosition,
                jobAgent.endPoint,
                ids,
                _navMesh.Triangles,
                _navMesh.Vertices(),
                _navMesh.SimpleVertices,
                agent);
        }

        private static Vector2Int GroupingIDByPosition(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(Mathf.Clamp(
                    (position.x - _navMesh.GetMinX()) / _navMesh.GetGroupDivisionSize(),
                    0,
                    _groupedUnitAgents.GetLength(0) - 1)),
                Mathf.FloorToInt(Mathf.Clamp(
                    (position.z - _navMesh.GetMinZ()) / _navMesh.GetGroupDivisionSize(),
                    0,
                    _groupedUnitAgents.GetLength(1) - 1)));
        }

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