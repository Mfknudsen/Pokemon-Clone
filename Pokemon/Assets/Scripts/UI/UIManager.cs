#region Packages

using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Battle.UI.Selection;
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

    public class UIManager : MonoBehaviour
    {
        #region Values

        public static UIManager instance;

        [Space] [SerializeField] private GameObject battleUI;
        [SerializeField] private GameObject overworldUI, pauseUI, startUI;

        [Space, Header("Battle:")] [SerializeField]
        private SelectionMenu selectionMenu;

        [SerializeField] private DisplayManager displayManager;

        #endregion

        #region Build In States

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(this);
        }

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
                    break;

                case UISelection.Box:
                    break;
            }
        }

        public void StartTestBattle()
        {
            FindObjectOfType<BattleStarter>().StartBattleNow();
        }

        #endregion
    }
}