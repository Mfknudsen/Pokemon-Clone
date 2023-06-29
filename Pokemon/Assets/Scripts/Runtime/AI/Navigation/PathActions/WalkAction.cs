#region Libraries

using Runtime.Common;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    public sealed class WalkAction : PathAction
    {
        #region Values

        public readonly Vector3 destination;

        #endregion

        #region Build In States

        public WalkAction(Vector3 destination) =>
            this.destination = destination;

        #endregion

        #region In

        public override bool PerformAction(UnitAgent agent)
        {
            agent.MoveAgentBody(this.destination);
            return agent.transform.position.XZ().QuickSquareDistance(this.destination.XZ()) < agent.Settings.StoppingDistance * agent.Settings.StoppingDistance;
        }

        #endregion
    }
}