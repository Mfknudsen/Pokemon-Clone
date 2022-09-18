#region Packages

using System.Collections;
using Runtime.Cutscenes;
using Runtime.Systems;
using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Runtime.Timeline
{
    [CreateAssetMenu(menuName = "Manager/Cutscene")]
    public class CutsceneManager : Manager
    {
        #region Values

        private PlayableDirector director;

        private Cutscene currentPlaying;

        #endregion

        #region Build In States

        public override IEnumerator StartManager()
        {
//            director ??= GetComponent<PlayableDirector>();
            yield break;
        }

        #endregion

        #region In

        public void PlayCutscene(Cutscene toPlay)
        {
            if (toPlay == null)
                return;

            if (currentPlaying != null)
                currentPlaying.Disable();

            currentPlaying = toPlay;
            currentPlaying.Enable();
        }

        #endregion
    }
}