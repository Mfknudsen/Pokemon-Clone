#region Libraries

using System.Collections.Generic;
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

        private static CalculatedNavMesh NavMeshTriangulation = null;

        private static UnityAction OnNavMeshChanged;

        private static readonly Dictionary<int, Dictionary<Vector3, List<UnitNavigationAgent>>> queuedAgentPathRequest = new();

        #endregion

        #region Setters

        public static void SetNavMesh(CalculatedNavMesh set)
        {
            NavMeshTriangulation = set;

            OnNavMeshChanged.Invoke();
        }

        #endregion

        #region In

        public static void AddOnNavMeshChange(UnityAction action)
            => OnNavMeshChanged += action;

        public static void RemoveOnNavMeshChange(UnityAction action)
            => OnNavMeshChanged -= action;

        public static void QueueForPath(UnitNavigationAgent agent, Vector3 destination)
        {
            if (queuedAgentPathRequest.TryGetValue(agent.CurrentTriangleIndex, out Dictionary<Vector3, List<UnitNavigationAgent>> value))
            {
                if (value.TryGetValue(destination, out List<UnitNavigationAgent> agentList))
                    agentList.Add(agent);
                else
                    value.Add(destination, new List<UnitNavigationAgent>() { agent });
            }
            else
            {
                Dictionary<Vector3, List<UnitNavigationAgent>> toAdd = new()
                {
                    { destination, new List<UnitNavigationAgent>() { agent } }
                };
                queuedAgentPathRequest.Add(agent.CurrentTriangleIndex, toAdd);
            }
        }

        #endregion

        #region Internal

        [RuntimeInitializeOnLoadMethod()]
        private static void Initialize()
        {
            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.subSystemList[5].updateDelegate += CalculatePaths;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
        }

        private static void CalculatePaths()
        {
            foreach (int triID in queuedAgentPathRequest.Keys)
            {
                foreach (Vector3 destination in queuedAgentPathRequest[triID].Keys)
                {
                    List<UnitNavigationAgent> agents = new();
                    queuedAgentPathRequest[triID][destination].ForEach(a =>
                    {

                    });
                }
            }
        }

#if UNITY_EDITOR
        private static void OnExitPlayMode(PlayModeStateChange state)
        {
            if (!state.Equals(PlayModeStateChange.ExitingPlayMode))
                return;

            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.subSystemList[5].updateDelegate -= CalculatePaths;
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
        }
#endif

        #endregion
    }

    internal struct PathCalculator : IJobParallelFor
    {
        public void Execute(int index)
        {
        }
    }
}