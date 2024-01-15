#region Libraries

using System;
using System.Collections.Generic;
using Runtime.World.Overworld.Lights;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

#endregion

namespace Runtime.World
{
    #region Enums

    /// <summary>
    /// Total of 5 different times of day. 0 - 4
    /// </summary>
    public enum DayTime
    {
        Midnight,
        Morning,
        Evening,
        Afternoon,
        Night
    }

    #endregion

    public static class DayNight
    {
        #region Values

        /// <summary>
        /// Time in hours (00 - 24)
        /// </summary>
        private static float _currentTime;

        private const int REALTIME_MINUTES_TO_A_DAY = 60;

        private static List<DayTimeLight> lights;

        #endregion

        #region In

        public static void SetCurrentDayTime(float set)
        {
            _currentTime = set;
        }

        public static void SetCurrentDayTime(DayTime set)
        {
            _currentTime = DayTimeToNumber(set);
        }

        public static void AddLight(DayTimeLight light) =>
            lights.Add(light);

        public static void RemoveLight(DayTimeLight light) =>
            lights.Remove(light);

        #endregion

        #region Out

        public static int DayTimeToNumber(DayTime dayTime)
        {
            return dayTime switch
            {
                DayTime.Midnight => 0,
                DayTime.Morning => 6,
                DayTime.Evening => 12,
                DayTime.Afternoon => 16,
                DayTime.Night => 20,
                _ => throw new ArgumentOutOfRangeException(nameof(dayTime), dayTime, null)
            };
        }

        public static DayTime NumberToDayTime(float time) =>
            time < 6 ? DayTime.Midnight :
            time < 12 ? DayTime.Morning :
            time < 16 ? DayTime.Evening :
            time < 20 ? DayTime.Afternoon :
            DayTime.Night;

        #endregion

        #region Internal

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type != typeof(Update))
                    continue;

                playerLoop.subSystemList[i].updateDelegate += UpdateTimeLighting;

                break;
            }

            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);

            SetCurrentDayTime(12);

            lights = new List<DayTimeLight>();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Clean up on exiting play mode.
        /// </summary>
        /// <param name="state">State giving by Unity</param>
        private static void OnExitPlayMode(PlayModeStateChange state)
        {
            UnityEngine.LowLevel.PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type != typeof(Update))
                    continue;

                playerLoop.subSystemList[i].updateDelegate -= UpdateTimeLighting;

                break;
            }

            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
        }
#endif

        private static void UpdateTimeLighting()
        {
            _currentTime += 24f / REALTIME_MINUTES_TO_A_DAY * Time.deltaTime;

            if (_currentTime > 24f)
                _currentTime -= 24f;

            UpdateLightmap();
        }

        private static void UpdateLightmap()
        {
            DayTime currentDayTime = NumberToDayTime(_currentTime),
                nextDayTime = currentDayTime + 1 <= DayTime.Night ? currentDayTime + 1 : DayTime.Midnight;
            LightmapData[] current = GetLightMapData(currentDayTime),
                next = GetLightMapData(nextDayTime);

            List<LightmapData> result = new List<LightmapData>();

            int timeCurrent = DayTimeToNumber(currentDayTime), timeNext = DayTimeToNumber(nextDayTime);
            float t = (timeNext - timeCurrent) / 100 * _currentTime - timeCurrent;

            for (int i = 0; i < current.Length; i++)
            {
                LightmapData data = new LightmapData();

                data.lightmapColor = new Texture2D(current[i].lightmapColor.width, current[i].lightmapColor.height);

                for (int x = 0; x < current[i].lightmapColor.width; x++)
                {
                    for (int y = 0; y < current[i].lightmapColor.height; y++)
                    {
                        data.lightmapColor.SetPixel(x, y, Color.Lerp(
                            current[i].lightmapColor.GetPixel(x, y),
                            next[i].lightmapColor.GetPixel(x, y),
                            t
                        ));
                    }
                }

                result.Add(data);
            }

            LightmapSettings.lightmaps = result.ToArray();
        }

        private static LightmapData[] GetLightMapData(DayTime time)
        {
            return new LightmapData[0];
        }

        #endregion
    }
}