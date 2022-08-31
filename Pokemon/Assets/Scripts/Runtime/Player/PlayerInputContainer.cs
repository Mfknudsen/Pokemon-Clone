#region Packages

using UnityEngine;

#endregion

namespace Runtime.Player
{
    public class PlayerInputContainer
    {
        private Vector2 moveDir;
        private bool run;

        #region Getters

        public Vector2 GetMoveDirection()
        {
            return moveDir;
        }

        public bool GetRun()
        {
            return run;
        }
        
        #endregion

        #region Setters

        public void SetMoveDirection(Vector2 set)
        {
            moveDir = set;
        }

        public void SetRun(bool set)
        {
            run = set;
        }
        
        #endregion
    }
}