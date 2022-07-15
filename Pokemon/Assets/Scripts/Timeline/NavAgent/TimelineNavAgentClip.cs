#region Packages

using Mfknudsen.Timeline.Mono;
using UnityEngine;

#endregion

namespace Mfknudsen.Timeline.NavAgent
{
    public class TimelineNavAgentClip : TimelineMonoClip<TimelineNavAgentBehaviour>
    {
        #region Values

        public Transform toMoveTo;
        public bool isStopped;
        public float stopDistance;

        #endregion

        #region Internal

        protected override void TransferValues(TimelineNavAgentBehaviour behaviour)
        {
            base.TransferValues(behaviour);

            behaviour.toMoveTo = toMoveTo;
            behaviour.isStopped = isStopped;
            behaviour.stopDistance = stopDistance;
        }

        #endregion
    }
}