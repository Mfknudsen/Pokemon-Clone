#region Packages

using Mfknudsen.Communication;
using UnityEngine.Playables;

#endregion

namespace Mfknudsen.Timeline.Chat
{
    public class TimelineChatBehaviour : PlayableBehaviour
    {
        #region Values

        public Communication.Chat clip;

        #endregion

        #region Build In States

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            
            ChatManager.instance.Add(clip);
        }

        #endregion
    }
}
