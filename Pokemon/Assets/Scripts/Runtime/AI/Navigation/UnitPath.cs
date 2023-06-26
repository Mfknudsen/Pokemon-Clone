#region Libraries

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    public struct UnitPath
    {
        #region Values

        private int actionIndex;
        private readonly List<PathAction> actions;

        private Vector3 destination;

        #endregion

        #region Build In States

        public UnitPath(Vector3 destination)
        {
            this.actionIndex = 0;
            this.destination = destination;

            this.actions = new();
        }

        #endregion

        #region Getters

        public Vector3 Destination() => this.destination;


        #endregion

        #region Setters

        public void AddActions(PathAction[] add)
        {
            List<PathAction> result = this.actions;
            this.actions.AddRange(add.Where(a => !result.Contains(a)));
        }

        #endregion

        #region Out

        public readonly bool Empty => this.actions?.Count == 0;

        #endregion
    }

}