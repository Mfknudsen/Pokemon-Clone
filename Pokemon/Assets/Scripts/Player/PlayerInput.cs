using UnityEngine;

namespace Mfknudsen.Player
{
    public struct PlayerInput
    {
        private Vector3 moveDir;
        private Vector3 targetRotation;

        #region Getters

        public Vector3 GetMoveDirection()
        {
            return moveDir;
        }

        public Vector3 GetTargetRotation()
        {
            return targetRotation;
        }

        #endregion
        
        #region Setters

        #region Move Direction

        public void SetMoveDirection(Vector2 set)
        {
            moveDir.x = set.x;
            moveDir.y = 0;
            moveDir.z = set.y;
        }

        public void SetMoveDirection(Vector3 set)
        {
            moveDir = set;
        }

        #endregion

        public void SetTargetRotation(Vector3 set)
        {
            targetRotation = set;
        }

        #endregion
    }
}
