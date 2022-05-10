#region Packages

using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

#endregion

namespace Mfknudsen.Timeline.Mono
{
    [DisplayName("Custom/Monobehaviour")]
    [TrackClipType(typeof(TimelineMonoClip<TimelineMonoBehaviour>))]
    [TrackBindingType(typeof(MonoBehaviour))]
    public class TimelineMonoTrack : TrackAsset
    {
    }
}
