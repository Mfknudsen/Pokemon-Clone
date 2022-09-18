#region Packages

using Runtime.Battle.UI.Information_Display;
using Runtime.Battle.UI.Selection;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Systems.UI
{
    public class UIController : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private UIManager uiManager;

        [SerializeField, Required, SceneObjectsOnly]
        private GameObject battleUI, overworldUI, pauseUI, startUI, loadingUI;

        [SerializeField, Required, SceneObjectsOnly]
        private SelectionMenu selectionMenu;

        [SerializeField, Required, SceneObjectsOnly]
        private DisplayManager displayManager;

        #endregion

        #region Build In States

        private void Start() => this.uiManager.Setup(
            this.battleUI,
            this.overworldUI,
            this.pauseUI,
            this.startUI,
            this.loadingUI,
            this.selectionMenu,
            this.displayManager);

        #endregion
    }
}