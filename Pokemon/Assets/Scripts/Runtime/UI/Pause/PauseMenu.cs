#region Packages

using JetBrains.Annotations;
using Runtime.Systems.UI;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.UI.Pause
{
    public class PauseMenu : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private UIManager uiManager;
        [SerializeField] private GameObject defaultPage, optionsPage, savePage, exitPage;

        private UISelection preSelection;
        private bool showMouse;

        #endregion

        #region In

        public void OnDisplay(UISelection toReturnSelection)
        {
            //Cursor.visible = !showMouse;
            //Cursor.lockState = showMouse ? CursorLockMode.Confined : CursorLockMode.None;

            showMouse = !showMouse;
            
            if (toReturnSelection != UISelection.Pause)
                preSelection = toReturnSelection;

            defaultPage.SetActive(true);
            optionsPage.SetActive(false);
            savePage.SetActive(false);
            exitPage.SetActive(false);
        }

        #region Navigation

        [UsedImplicitly]
        public void Unpause()
        {
            uiManager.SwitchUI(preSelection);
        }

        [UsedImplicitly]
        public void ActivateOptions(bool toActivate)
        {
            defaultPage.SetActive(!toActivate);
            optionsPage.SetActive(toActivate);

            optionsPage.GetComponent<Options>().Gameplay();
        }


        [UsedImplicitly]
        public void ActivateSave(bool toActivate)
        {
            defaultPage.SetActive(!toActivate);
            savePage.SetActive(toActivate);
        }


        [UsedImplicitly]
        public void Exit()
        {
            exitPage.SetActive(true);
        }

        #endregion

        #endregion
    }
}