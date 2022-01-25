#region Packages

using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Mfknudsen.UI.Pause
{
    public class Exit : MonoBehaviour
    {
        #region In

        [UsedImplicitly]
        public void Cancel()
        {
            gameObject.SetActive(false);
        }

        [UsedImplicitly]
        public void Accept()
        {
            Application.Quit();
        }

        #endregion
    }
}
