#region Packages

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.Common
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
            return this.stop;
        }

        #endregion

        #region In

        public void Update()
        {
            if (this.done || this.stop) return;

            this.current += Time.deltaTime;

            if (!(this.current >= this.duration)) return;

            this.timerEvent.Invoke();
            this.done = true;
        }

        public void Stop()
        {
            this.stop = true;
        }

        #endregion
    }
}