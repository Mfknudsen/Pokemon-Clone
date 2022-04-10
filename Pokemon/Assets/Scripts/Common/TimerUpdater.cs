#region Packages

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mfknudsen.Common
{
    public class TimerUpdater : MonoBehaviour
    {
        public static TimerUpdater instance;
        public List<Timer> timers = new List<Timer>();

        private void Start()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            Timer[] toUpdate = timers.Where(t => t != null).ToArray();
            List<Timer> toRemove = new List<Timer>();

            foreach (Timer timer in toUpdate.Where(t => t != null))
            {
                if (timer.GetStopped())
                    toRemove.Add(timer);
                else
                    timer.Update();
            }

            toRemove.ForEach(t => timers.Remove(t));
        }
    }
}