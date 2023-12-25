#region Libraries

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Core
{
    public sealed class TimerUpdater : MonoBehaviour
    {
        #region Values

        private static readonly List<Timer> Timers = new List<Timer>();

        private static TimerUpdater _instance;

        #endregion

        #region Build In States

        private void OnDestroy()
        {
            foreach (Timer t in Timers) 
                t.Stop();
            
            Timers.Clear();
            _instance = null;
        }

        private void Update()
        {
            for (int index = Timers.Count - 1; index >= 0; index--)
            {
                if (Timers[index].IsDone)
                    Timers.RemoveAt(index);
                else
                    Timers[index].Update();
            }
        }

        #endregion

        #region In

        internal static void Add(Timer toAdd)
        {
            if (toAdd == null)
                return;

            if (_instance == null)
            {
                GameObject obj = new GameObject("Timer Updater");
                _instance = obj.AddComponent<TimerUpdater>();
                DontDestroyOnLoad(obj);
            }

            if (!Timers.Contains(toAdd))
                Timers.Add(toAdd);
        }

        #endregion
    }
}