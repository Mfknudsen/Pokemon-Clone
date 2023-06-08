#region Libraries

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.AI.World.Navigation
{
    public sealed class UnitPath
    {
        #region Values

        private int currentWaypointIndex = 0;
        private Vector3[] waypoints = Array.Empty<Vector3>();
        private List<Interaction> interactions = new();

        #endregion

        #region Out

        public Vector3 Destination() => this.waypoints[^1];

        #endregion
    }

    internal struct Interaction
    {
        public int onWaypointIndex, interactionIndex;

        public Interaction(int onWaypointIndex, int interactionIndex)
        {
            this.onWaypointIndex = onWaypointIndex;
            this.interactionIndex = interactionIndex;
        }
    }
}