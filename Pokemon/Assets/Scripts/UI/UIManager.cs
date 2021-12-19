#region Packages

using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Player;
using Mfknudsen.Player.UI_Book;
using Mfknudsen.Settings.Manager;
using Mfknudsen.UI.Pause;
using UnityEngine;
using UnityEngine.UI;

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

        #region In

        public override void Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                InputManager.instance.pauseInputEvent.AddListener(PauseTrigger);
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
            
            if(selection == UISelection.Overworld)
                PlayerManager.instance.EnablePlayerControl();
            else 
                PlayerManager.instance.DisablePlayerControl();
        }

        public void ActivateLoadingUI(bool set)
        {
            loadingUI.SetActive(set);
        }
        
        #endregion

        #region Internal

        private void PauseTrigger()
        {
            SwitchUI(UISelection.Pause);
            
            UIBook.instance.Effect(BookTurn.Open);
        }

        #endregion
    }
}