#region Packages

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mfknudsen.Common
{
    public class Timer
    {
        #region Values

        public readonly UnityEvent timerEvent = new();
        private readonly float duration;
        private float current;
        private bool done, stop;

        #endregion

        #region Build In States

        public Timer(float duration)
        {
            TimerUpdater.instance.timers.Add(this);
            this.duration = duration;
        }

        #endregion

        #region Getters

        public bool GetStopped()
        {
            return stop;
        }

        #endregion

        #region In

        public void Update()
        {
            if (done || stop) return;

            current += Time.deltaTime;

            if (!(current >= duration)) return;
            
            timerEvent.Invoke();
            done = true;
        }

        public void Stop()
        {
            stop = true;
        }

        #endregion
    }
}