#region Packages

using Runtime.Battle.Systems;
using Runtime.Battle.UI.Information_Display;
using Runtime.Battle.UI.Selection;
using Runtime.ScriptableVariables.Structs;
using Runtime.Systems;
using Runtime.UI_Book;
using Runtime.UI.Pause;
using UnityEngine;

#endregion

namespace Runtime.UI
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

    public class UIManager : Manager
    {
        #region Values

        [SerializeField] private GameObject battleUI, overworldUI, pauseUI, startUI, loadingUI;

        [SerializeField] private SelectionMenu selectionMenu;

        [SerializeField] private DisplayManager displayManager;

        [SerializeField] private BoolVariable playerThrowingItem;

        private UISelection currentSelection = UISelection.Start;
        private bool readyToPause;

        #endregion

        #region Build In States

        private void OnEnable() => InputManager.instance.pauseInputEvent.AddListener(PauseTrigger);
        private void OnDisable() => InputManager.instance.pauseInputEvent.RemoveListener(PauseTrigger);

        #endregion

        #region Getters

        #region Battle

        public SelectionMenu GetSelectionMenu()
        {
            return selectionMenu;
        }

        public DisplayManager GetDisplayManager()
        {
            return displayManager;
        }

        #endregion

        #endregion

        #region Setters

        public void SetReadyToPause(bool set)
        {
            readyToPause = set;
        }

        #endregion

        #region In

        public void SwitchUI(UISelection selection)
        {
            battleUI.SetActive(false);
            overworldUI.SetActive(false);
            pauseUI.SetActive(false);
            startUI.SetActive(false);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (selection)
            {
                case UISelection.Start:
                    startUI.SetActive(true);
                    break;

                case UISelection.Battle:
                    battleUI.SetActive(true);
                    break;

                case UISelection.Overworld:
                    overworldUI.SetActive(true);
                    break;

                case UISelection.Pause:
                    pauseUI.SetActive(true);
                    pauseUI.GetComponent<PauseMenu>().OnDisplay(currentSelection);
                    break;

                case UISelection.Box:
                    break;
            }

            currentSelection = selection;
        }

        public void ActivateLoadingUI(bool set)
        {
            loadingUI.SetActive(set);
        }

        #endregion

        #region Internal

        private void PauseTrigger()
        {
            if (this.playerThrowingItem.value) return;

            if (BattleManager.instance != null) return;

            if (!this.readyToPause) return;

            this.readyToPause = false;

            SwitchUI(UISelection.Pause);

            UIBook.instance.Effect(BookTurn.Open);
        }

        #endregion
    }
}