#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.Player
{
    public class PlayerInputContainer
    {
        private Vector2 moveDir;

        #region Getters

        public Vector2 GetMoveDirection()
        {
            return moveDir;
        }

        #endregion

        #region Setters

        public void SetMoveDirection(Vector2 set)
        {
            moveDir = set;
        }

        #endregion
    }
}