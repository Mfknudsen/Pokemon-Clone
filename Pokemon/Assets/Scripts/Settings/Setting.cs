#region Packages

using Mfknudsen.Player.Camera;
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

        public static int ResolutionIndex = 1;

        public static readonly Vector2[] Resolutions =
            { new Vector2(1280, 720), new Vector2(1920, 1080), new Vector2(2560, 1440) };

        public static float MasterSoundLevel = 50, MusicLevel = 50, AmbientLevel = 50;
        public static FullScreenMode ScreenSetting = FullScreenMode.FullScreenWindow;

        public static CameraSettings OverworldCameraSettings, BattleCameraSettings;
    }
}