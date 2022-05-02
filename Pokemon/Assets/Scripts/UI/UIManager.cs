#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Player;
using Mfknudsen.Player.UI_Book;
using Mfknudsen.Settings.Manager;
using Mfknudsen.UI.Pause;
using UnityEngine;

#endregion

namespace Mfknudsen.UI
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

        public static UIManager instance;

        [SerializeField] private GameObject battleUI, overworldUI, pauseUI, startUI, loadingUI;

        [SerializeField] private SelectionMenu selectionMenu;

        [SerializeField] private DisplayManager displayManager;

        private UISelection currentSelection = UISelection.Start;
        private bool readyToPause;

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

        public override IEnumerator Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                while (InputManager.Instance == null)
                    yield return null;

                InputManager.Instance.pauseInputEvent.AddListener(PauseTrigger);
            }
            else
                Destroy(gameObject);
        }

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
            if (BattleManager.instance != null) return;

            if (!readyToPause) return;

            readyToPause = false;

            SwitchUI(UISelection.Pause);

            UIBook.instance.Effect(BookTurn.Open);
        }

        #endregion
    }
}