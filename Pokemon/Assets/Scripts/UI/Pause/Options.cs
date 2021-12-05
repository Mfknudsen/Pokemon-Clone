#region Packages

using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Pause
{
    public class Options : MonoBehaviour
    {
        #region Values

        [SerializeField] private GameObject gameplayTab, audioTab, screenTab;

        #endregion

        #region In

        [UsedImplicitly]
        public void Gameplay()
        {
            gameplayTab.SetActive(true);

            audioTab.SetActive(false);
            screenTab.SetActive(false);
        }

        [UsedImplicitly]
        public void Audio()
        {
            audioTab.SetActive(true);

            gameplayTab.SetActive(false);
            screenTab.SetActive(false);
        }

        [UsedImplicitly]
        public void Screen()
        {
            screenTab.SetActive(true);

            gameplayTab.SetActive(false);
            audioTab.SetActive(false);
        }

        #endregion
    }
}