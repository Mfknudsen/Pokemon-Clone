#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.Player
{
    public class CamController : MonoBehaviour
    {
        #region Values

        [SerializeField] private Transform lookTransform;

        private PlayerInputContainer playerInputContainer;

        #endregion

        #region Build In States

        private void Update()
        {
        }

        #endregion

        #region Getters

        #endregion

        #region Setters

        #endregion

        #region In

        public void Setup()
        {
            playerInputContainer = PlayerManager.instance.GetPlayerInput();
        }

        #endregion

        #region Out

        #endregion

        #region Internal

        #endregion
    }
}