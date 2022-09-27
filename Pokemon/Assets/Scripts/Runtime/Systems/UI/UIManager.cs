#region Packages

using System.Collections;
using Runtime.Battle.Systems;
using Runtime.Battle.UI.Information_Display;
using Runtime.Battle.UI.Selection;
using Runtime.ScriptableVariables.Structs;
using Runtime.UI_Book;
using Runtime.UI.Pause;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Systems.UI
{
    #region Enums

    public enum UISelection
    {
        Start,
        Battle,
        Overworld,
        Pause,
        Box
    }

    #endregion

    [CreateAssetMenu(menuName = "Manager/UI")]
    public class UIManager : Manager
    {
        #region Values

        private UIBook uiBook;

        [ShowInInspector, ReadOnly] private GameObject battleUI, overworldUI, pauseUI, startUI, loadingUI;

        [ShowInInspector, ReadOnly] private SelectionMenu selectionMenu;

        [ShowInInspector, ReadOnly] private DisplayManager displayManager;

        [SerializeField, Required] private BoolVariable playerThrowingItem;

        private UISelection currentSelection = UISelection.Start;
        private bool readyToPause;

        #endregion

        #region Build In States

        public override IEnumerator StartManager()
        {
            InputManager.instance.pauseInputEvent.AddListener(PauseTrigger);
            yield break;
        }

        private void OnDisable() => InputManager.instance.pauseInputEvent.RemoveListener(PauseTrigger);

        #endregion

        #region Getters

        public UIBook UIBook => this.uiBook;

        #region Battle

        public SelectionMenu GetSelectionMenu()
        {
            return this.selectionMenu;
        }

        public DisplayManager GetDisplayManager()
        {
            return this.displayManager;
        }

        #endregion

        #endregion

        #region Setters

        public void SetReadyToPause(bool set) => this.readyToPause = set;

        public void SetUIBook(UIBook set) => this.uiBook = set;

        #endregion

        #region In

        public void Setup(GameObject battleUI, GameObject overworldUI, GameObject pauseUI, GameObject startUI,
            GameObject loadingUI, SelectionMenu selectionMenu, DisplayManager displayManager)
        {
            this.battleUI = battleUI;
            this.overworldUI = overworldUI;
            this.pauseUI = pauseUI;
            this.startUI = startUI;
            this.loadingUI = loadingUI;
            this.selectionMenu = selectionMenu;
            this.displayManager = displayManager;
        }

        public void SwitchUI(UISelection selection)
        {
            this.battleUI.SetActive(false);
            this.overworldUI.SetActive(false);
            this.pauseUI.SetActive(false);
            this.startUI.SetActive(false);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (selection)
            {
                case UISelection.Start:
                    this.startUI.SetActive(true);
                    break;

                case UISelection.Battle:
                    this.battleUI.SetActive(true);
                    break;

                case UISelection.Overworld:
                    this.overworldUI.SetActive(true);
                    break;

                case UISelection.Pause:
                    this.pauseUI.SetActive(true);
                    this.pauseUI.GetComponent<PauseMenu>().OnDisplay(this.currentSelection);
                    break;

                case UISelection.Box:
                    break;
            }

            this.currentSelection = selection;
        }

        public void ActivateLoadingUI(bool set)
        {
            this.loadingUI.SetActive(set);
        }

        #endregion

        #region Internal

        private void PauseTrigger()
        {
            if (this.playerThrowingItem.value) return;

            if (BattleManager.instance is not null) return;

            if (!this.readyToPause) return;

            this.readyToPause = false;

            SwitchUI(UISelection.Pause);

            this.uiBook.Effect(BookTurn.Open);
        }

        #endregion
    }
}