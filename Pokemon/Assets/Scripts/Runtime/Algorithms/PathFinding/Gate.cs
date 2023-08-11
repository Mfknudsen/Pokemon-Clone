#region Libraries

using System.Diagnostics;
using UnityEngine;

#endregion

namespace Runtime.Algorithms.PathFinding
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Gate
    {
        private string DebuggerDisplay =>
            $"Left: {this.left.x:0.00}, {this.left.y:0.00}  Right: {this.right.x:0.00}, {this.right.y:0.00}";

        public Vector3 left;
        public Vector3 right;
        public bool isGoalGate;
    }
}