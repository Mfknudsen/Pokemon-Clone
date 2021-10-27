#region Packages

using Mfknudsen.Settings;
using UnityEngine;

#endregion

namespace Mfknudsen.UI
{
    public class OptionsUI : MonoBehaviour
    {
        #region Values

        #endregion

        #region Build In State

        private void Awake()
        {
        }

        #endregion

        #region In

        public void ApplySettings()
        {
            #region Screen

            Vector2 res = Setting.resolutions[Setting.resolutionIndex];
            FullScreenMode mode = Setting.screenSetting;
            Screen.SetResolution((int)res.x, (int)res.y, mode);

            #endregion
        }

        public void SetDifficultly(Difficultly set)
        {
            Setting.Difficultly = set;
        }

        #region Screen

        public void SetResolution(int i)
        {
            Setting.resolutionIndex += i;
            if (Setting.resolutionIndex < 0)
                Setting.resolutionIndex = Setting.resolutions.Length - 1;
            else if (Setting.resolutionIndex >= Setting.resolutions.Length)
                Setting.resolutionIndex = 0;
        }

        #endregion

        #endregion

        #region Internal

        #endregion
    }
}