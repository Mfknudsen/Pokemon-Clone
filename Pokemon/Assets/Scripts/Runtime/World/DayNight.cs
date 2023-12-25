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
        }

        #endregion
    }
}