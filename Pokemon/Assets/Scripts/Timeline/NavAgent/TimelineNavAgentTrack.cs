#region Packages

using System.ComponentModel;
using Mfknudsen.Timeline.Mono;
using UnityEngine.AI;
using UnityEngine.Timeline;

#endregion

namespace Mfknudsen.Timeline.NavAgent
{
    [DisplayName("Custom/Navigation Agent")]
    [TrackClipType(typeof(TimelineNavAgentClip))]
    [TrackBindingType(typeof(NavMeshAgent))]
    public class TimelineNavAgentTrack : TimelineMonoTrack
    {
        
    }
}