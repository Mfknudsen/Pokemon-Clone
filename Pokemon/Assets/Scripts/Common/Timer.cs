#region Packages

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Mfknudsen.Common
{
    public class TimerEvent : UnityEvent
    {
    }

    public class Timer
    {
        public TimerEvent timerEvent;
        private float duration, current;
        private bool done;

        public Timer(float duration)
        {
            this.duration = duration;
        }

        public void Update()
        {
            if (done) return;

            current += Time.deltaTime;

            if (current >= duration)
            {
                timerEvent.Invoke();
                done = true;
            }
        }
    }
}