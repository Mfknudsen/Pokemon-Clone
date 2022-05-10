#region Packages

using System;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Common;
using Mfknudsen.Settings.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Mfknudsen.Cutscenes
{
    [CreateAssetMenu(menuName = "Cutscenes/Basic", fileName = "Basic Cutscene")]
    public class Cutscene : ScriptableObject
    {
        #region Values

        [SerializeField] private List<PlayableAsset> timelines;
        [SerializeField] private List<DelayTime> delayTimes;

        private int currentPlayIndex = 0;
        private bool readyForNext, waitingForInput;
        
        #endregion
        
        #region In

        public void Enable()
        {
            InputManager.Instance.nextChatInputEvent.AddListener(OnNextChatUpdate);
        }

        public void Disable()
        {
            InputManager.Instance.nextChatInputEvent.RemoveListener(OnNextChatUpdate);
        }
        
        public void DeployCutscene(){}

        public void TriggerNext()
        {
            if (delayTimes.Where(d => d.index.Equals(currentPlayIndex)).Select(d => d.delay).First() is float delayTime)
            {
                new Timer(delayTime).timerEvent.AddListener(PlayNext);
            }
            else
                waitingForInput = true;
            
            currentPlayIndex++;
        }

        #endregion

        #region Internal

        private void PlayNext()
        {
            
        }

        private void OnNextChatUpdate()
        {
            
        }

        #endregion
    }
    
    [Serializable]
    internal struct DelayTime
    {
        [HorizontalGroup(" ")] 
        public int index;
        [HorizontalGroup(" ")]
        public float delay;
    }
}
