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

        public static TimerUpdater instance;
        public static readonly List<Timer> timers = new();

        #endregion

        #region Build In States

        private void Start()
        {
            if (instance != null)
                Destroy(this.gameObject);

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnDestroy()
        {
            timers.ForEach(t => t.Stop());
            timers.Clear();
            instance = null;
        }

        private void Update()
        {
            Timer[] toUpdate = timers.Where(t => t != null).ToArray();
            List<Timer> toRemove = new();

            foreach (Timer timer in toUpdate.Where(t => t != null))
            {
                if (timer.GetStopped())
                    toRemove.Add(timer);
                else
                    timer.Update();
            }

            toRemove.ForEach(t => timers.Remove(t));
        }

        #endregion
    }
}