#region Packages

using Mfknudsen.Cutscenes;
using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Mfknudsen.Timeline
{
    public class CutsceneManager : MonoBehaviour
    {
        #region Values

        public static CutsceneManager instance;
        
        private PlayableDirector director;

        private Cutscene currentPlaying;
        
        #endregion

        #region Build In States

        private void Start()
        {
            if(instance != null)
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);
            
            director ??= GetComponent<PlayableDirector>();
        }

        #endregion

        #region In

        public void PlayCutscene(Cutscene toPlay)
        {
            if(toPlay == null) 
                return;
            
            if(currentPlaying != null)
                currentPlaying.Disable();
            
            currentPlaying = toPlay;
            currentPlaying.Enable();
        }

        #endregion
    }
}
