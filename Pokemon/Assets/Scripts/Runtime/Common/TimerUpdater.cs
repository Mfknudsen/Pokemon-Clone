#region Packages

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Runtime.Common
{
    public class TimerUpdater : MonoBehaviour
    {
        public static TimerUpdater instance;
        public readonly List<Timer> timers = new();

        private void Start()
        {
            if (instance != null)
                Destroy(this.gameObject);

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            Timer[] toUpdate = this.timers.Where(t => t != null).ToArray();
            List<Timer> toRemove = new();

            foreach (Timer timer in toUpdate.Where(t => t != null))
            {
                if (timer.GetStopped())
                    toRemove.Add(timer);
                else
                    timer.Update();
            }

            toRemove.ForEach(t => this.timers.Remove(t));
        }
    }
}