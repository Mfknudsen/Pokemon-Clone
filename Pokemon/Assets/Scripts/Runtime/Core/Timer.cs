#region Libraries

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.Core
{
    public sealed class Timer
    {
        #region Values

        private readonly UnityEvent timerEvent = new UnityEvent();
        private readonly float duration;
        private float current;
        private bool done;

        #endregion

        #region Build In States

        public Timer(float duration, UnityAction action = null)
        {
            this.duration = duration;
            this.timerEvent.AddListener(action);
            TimerUpdater.Add(this);
        }

        #endregion

        #region Getters

        public bool IsDone => this.done;

        #endregion

        #region In

        public void Update()
        {
            if (this.done) return;

            this.current += Time.deltaTime;

            if (this.current < this.duration) return;

            this.timerEvent.Invoke();
            this.done = true;
        }

        public void Stop() =>
            this.done = true;

        #endregion
    }
}