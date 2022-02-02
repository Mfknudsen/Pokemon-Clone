#region Packages

using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.UI.Cursor
{
    public class CustomCursor : MonoBehaviour
    {
        #region Values

        public static CustomCursor instance;

        [SerializeField] private GameObject visual;

        #endregion

        #region Build In States

        private void Start()
        {
            instance = this;
        }

        private void Update()
        {
            OnTurnAxisChange(Mouse.current.position.ReadValue());
        }

        #endregion

        #region In

        public static void ShowCursor()
        {
            instance.visual.SetActive(true);
        }

        public static void HideCursor()
        {
            instance.visual.SetActive(false);
        }

        #endregion

        #region Internal

        private void OnTurnAxisChange(Vector2 input)
        {
            transform.position = Vector3.Lerp(transform.position, input, 0.9f);
        }

        #endregion
    }
}