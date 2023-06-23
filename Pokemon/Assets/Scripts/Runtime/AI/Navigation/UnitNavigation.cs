#region Libraries

using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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

        public static bool ready => NavMesh != null;

        private static NativeArray<Vector3> verts;
        private static NativeArray<int> inds, areas;
        private static NativeArray<JobTriangle> triangles;

        private static bool switchNativeArrays;

        #endregion

        #region Setters

        public static bool SetNavMesh(CalculatedNavMesh set)
        {
            if (NavMesh == set)
                return false;

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

        public static void QueueForPath(UnitNavigationAgent agent, Vector3 destination)
        {

        }

        #endregion

        #region Out

        public static int ClosestTriangleIndex(Vector2 p) => NavMesh.ClosestTriangleIndex(p);

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

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
        }

        private static void Update()
        {
            CalculatePaths();

            UpdateNativeArrays();
        }

        private static void UpdateNativeArrays()
        {
            if (!switchNativeArrays)
                return;

            if (verts.Length > 0)
                verts.Dispose();
            Vector3[] navVerts = NavMesh.Vertices();
            verts = new NativeArray<Vector3>(navVerts, Allocator.Persistent);


            if (areas.Length > 0)
                areas.Dispose();
            int[] navAreas = NavMesh.Areas;
            areas = new NativeArray<int>(navAreas, Allocator.Persistent);


            if (inds.Length > 0)
                inds.Dispose();
            int[] navInds = NavMesh.Inds;
            inds = new NativeArray<int>(navInds, Allocator.Persistent);


            if (triangles.Length > 0)
                triangles.Dispose();
            NavTriangle[] navTriangles = NavMesh.Triangles.ToArray();
            triangles = new NativeArray<JobTriangle>(navTriangles.Length, Allocator.Persistent);
            for (int i = 0; i < navTriangles.Length; i++)
                triangles[i] = new JobTriangle(navTriangles[i]);

            switchNativeArrays = false;
        }

        private static void CalculatePaths()
        {


        }

#if UNITY_EDITOR
        private static void OnExitPlayMode(PlayModeStateChange state)
        {
            if (!state.Equals(PlayModeStateChange.ExitingPlayMode))
                return;

            if (verts.Length > 0)
                verts.Dispose();
            if (areas.Length > 0)
                areas.Dispose();
            if (inds.Length > 0)
                inds.Dispose();
            if (triangles.Length > 0)
                triangles.Dispose();

            NavMesh = null;

            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.subSystemList[5].updateDelegate -= Update;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
        }
#endif

        #endregion
    }

    internal readonly struct JobTriangle
    {
        #region Values

        public readonly int id, a, b, c, area;

        public readonly NativeArray<int> neighbors;

        #endregion

        public JobTriangle(NavTriangle triangle)
        {
            this.id = triangle.ID;
            this.a = triangle.Vertices[0];
            this.b = triangle.Vertices[1];
            this.c = triangle.Vertices[2];
            this.area = triangle.Area;
            this.neighbors = new NativeArray<int>(triangle.Neighbors.ToArray(), Allocator.Persistent);
        }
    }

    [BurstCompile]
    internal struct PathCalculatorJob : IJobParallelFor
    {
        private NativeArray<UnitPath> paths;

        public void Execute(int index)
        {
        }
    }
}