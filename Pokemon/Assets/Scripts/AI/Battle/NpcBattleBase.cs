#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.AI.Battle.AfterEffects;
using Mfknudsen.AI.Battle.BeforeEffects;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Battle
{
    public class NpcBattleBase : NpcBase
    {
        #region Values

        [FoldoutGroup("Before Battle")] [SerializeField]
        private BattleStarter battleStarter;

        [FoldoutGroup("Before Battle")] [SerializeField]
        private BeforeEffectContainer[] beforeEffects = new BeforeEffectContainer[0];

        [FoldoutGroup("Before Battle")] [SerializeField]
        private AfterEffectContainer[] afterEffects = new AfterEffectContainer[0];

        [FoldoutGroup("Input")] [SerializeField]
        private string[] chatKeys, chatValues;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            beforeEffects.ForEach(e =>
                e.OnValidate());
            afterEffects.ForEach(e =>
                e.OnValidate());
        }

        private void Start()
        {
            //Setup Input
            Dictionary<string, string> chatInput = SetupChatInput();

            //Apply
            foreach (BeforeEffectContainer container in beforeEffects)
            {
                container.SetInput<TriggerBeforeChat>(chatInput);
            }

            foreach (AfterEffectContainer container in afterEffects)
            {
                container.SetInput<TriggerAfterChat>(chatInput);
            }
        }

        #endregion

        #region In

        public override void Trigger()
        {
            if (battleStarter == null) return;

            if (!battleStarter.GetPlayerWon())
                StartCoroutine(BeforeBattle());
            else
                ChatManager.instance.Add(idleChat);
        }

        #endregion

        #region Internal

        #region Setup Input

        private Dictionary<string, string> SetupChatInput()
        {
            if (chatKeys == null)
                return null;
            
            Dictionary<string, string> chatInput = new Dictionary<string, string>();
            for (int i = 0; i < chatKeys.Length; i++)
            {
                if (i < chatValues.Length)
                    chatInput.Add(chatKeys[i], chatValues[i]);
            }

            return chatInput;
        }

        #endregion

        private IEnumerator BeforeBattle()
        {
            foreach (BeforeEffectContainer container in beforeEffects)
            {
                container.Trigger();
                yield return null;
                yield return new WaitWhile(() => !container.AllDone());
            }

            battleStarter.StartBattleNow();
        }

        public IEnumerator AfterBattle()
        {
            foreach (AfterEffectContainer container in afterEffects)
            {
                container.Trigger();
                yield return null;
                yield return new WaitWhile(() => !container.AllDone());
            }
        }

        #endregion
    }
}