#region Packages

using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Common;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Runtime.Cutscenes
{
    [CreateAssetMenu(menuName = "Cutscenes/Basic", fileName = "Basic Cutscene")]
    public class Cutscene : ScriptableObject
    {
        #region Values

        [SerializeField] private List<PlayableAsset> timelines;
        [SerializeField] private List<DelayTime> delayTimes;

        private int currentPlayIndex;
        private bool readyForNext, waitingForInput;
        
        #endregion
        
        #region In

        public void Enable()
        {
            InputManager.instance.nextChatInputEvent.AddListener(OnNextChatUpdate);
        }

        public void Disable()
        {
            InputManager.instance.nextChatInputEvent.RemoveListener(OnNextChatUpdate);
        }
        
        public void DeployCutscene(){}

        public void TriggerNext()
        {
            if (this.delayTimes.Where(d => d.index.Equals(this.currentPlayIndex)).Select(d => d.delay).First() is float delayTime)
            {
                new Timer(delayTime).timerEvent.AddListener(PlayNext);
            }
            else
                this.waitingForInput = true;

            this.currentPlayIndex++;
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
