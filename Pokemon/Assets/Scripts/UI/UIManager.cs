#region SDK

using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Battle.UI.Selection;
using UnityEngine;

#endregion

namespace Mfknudsen.UI
{
    public enum UISelection
    {
        Start,
        Battle,
        Overworld,
        Pause,
        Box
    }

    public class UIManager : MonoBehaviour
    {
        #region Values

        public static UIManager instance;

        [SerializeField] private GameObject battleUI, overworldUI, pauseUI, startUI;

        [Space, Header("Battle:")] [SerializeField]
        private SelectionMenu selectionMenu;

        [SerializeField] private DisplayManager displayManager;

        #endregion

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

        #endregion
    }
}