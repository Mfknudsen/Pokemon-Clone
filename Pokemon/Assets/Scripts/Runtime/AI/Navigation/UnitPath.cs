#region Libraries

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    public sealed class UnitPath
    {
        #region Values

        private int actionIndex = 0;
        private List<PathAction> actions = new();

        private Vector3 destination;

        #endregion

        #region Out

        public Vector3 Destination() => this.destination;

        #endregion
    }

}