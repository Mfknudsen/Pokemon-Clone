#region Libraries

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Runtime.Common
{
    public sealed class TimerUpdater : MonoBehaviour
    {
        #region Values

        private static readonly List<Timer> timers = new();

        private static TimerUpdater instance;

        #endregion

        #region Build In States

        private void OnDestroy()
        {
            timers.ForEach(t => t.Stop());
            timers.Clear();
            instance = null;
        }

        private void Update()
        {
            List<Timer> toUpdate = timers.ToList(), toRemove = new();

            for (int i = 0; i < toUpdate.Count; i++)
            {
                Timer timer = toUpdate[i];

                if (timer.IsDone)
                    toRemove.Add(timer);
                else
                    timer.Update();
            }

            toRemove.ForEach(t => timers.Remove(t));
        }

        #endregion

        #region In

        public static void Add(Timer toAdd)
        {
            if (toAdd == null)
                return;

            if (instance == null)
            {
                GameObject obj = new("Timer Updator");
                instance = obj.AddComponent<TimerUpdater>();
                DontDestroyOnLoad(obj);
            }

            if (!timers.Contains(toAdd))
                timers.Add(toAdd);
        }

        #endregion
    }
}