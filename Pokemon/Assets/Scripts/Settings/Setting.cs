#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.Settings
{
    #region Enum

    public enum ScreenSetting
    {
        Window,
        Fullscreen,
        Borderless
    }

    public enum Difficultly
    {
        Easy,
        Medium,
        Hard
    }

    #endregion

    public static class Setting
    {
        public static Difficultly Difficultly = Difficultly.Easy;

        public static int resolutionIndex = 1;

        public static readonly Vector2[] resolutions =
            { new Vector2(1280, 720), new Vector2(1920, 1080), new Vector2(2560, 1440) };

        public static float masterSoundLevel = 50, musicLevel = 50, ambientLevel = 50;
        public static FullScreenMode screenSetting = FullScreenMode.FullScreenWindow;
    }
}