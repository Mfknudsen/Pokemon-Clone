#region Libraries


#endregion

using UnityEngine;

namespace Runtime.AI.Navigation.PathActions
{
    public sealed class InteractAction : PathAction
    {
        public override Vector3 Destination()
        {
            throw new System.NotImplementedException();
        }

        public override bool CheckAction(UnitAgent agent)
        {
            throw new System.NotImplementedException();
        }

        #region Getters

        public override bool IsWalkAction() => false;

        #endregion
    }
}