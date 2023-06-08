#region Libraries

using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine.AI;
using UnityEngine.Events;

#endregion

namespace Runtime.AI.World.Navigation
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

        private static NavMeshTriangulation NavMeshTriangulation;

        private static UnityAction<NavMeshTriangulation> OnNavMeshChanged;

        #endregion

        #region Setters

        public static void SetNavMesh(NavMeshTriangulation set)
        {
            NavMeshTriangulation = set;

            OnNavMeshChanged.Invoke(NavMeshTriangulation);
        }

        #endregion

        #region In

        public static void AddOnNavMeshChange(UnityAction<NavMeshTriangulation> action)
            => OnNavMeshChanged += action;

        public static void RemoveOnNavMeshChange(UnityAction<NavMeshTriangulation> action)
            => OnNavMeshChanged -= action;

        public static UnitPath CalculatePath()
        {
            UnitPath path = new();
            return path;
        }

        #endregion
    }

    internal struct PathCalculator : IJobParallelFor
    {
        public void Execute(int index)
        {
        }
    }

    internal struct PathTriangle
    {
        private readonly int A, B, C;
        private List<PathTriangle> neightbors;
    }

    internal struct PathInteraction
    {
        private readonly int A, B;
    }
}