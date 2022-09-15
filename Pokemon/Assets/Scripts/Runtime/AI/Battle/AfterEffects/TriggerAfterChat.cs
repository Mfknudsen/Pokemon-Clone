#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Communication;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle.AfterEffects
{
    [Serializable]
    public class TriggerAfterChat : NpcBattleAfterEffect
    {
        [SerializeField] private ChatManager chatManager;
        [SerializeField] private Chat chat;

        private Dictionary<string, string> input;

        #region In

        public override void SetInput(object i)
        {
            try
            {
                input = i as Dictionary<string, string>;
            }
            catch (Exception e)
            {
                Debug.LogError("Input was not Dictionary of string/string\n" + e.Message);
                input = null;
            }
        }

        public override IEnumerator Operation()
        {
            if (input == null) yield break;

            // ReSharper disable once AccessToStaticMemberViaDerivedType
            chat = Instantiate(chat);
            foreach (string inputKey in input.Keys)
                chat.AddToOverride(
                    inputKey,
                    input[inputKey]);

            chatManager.Add(chat);

            yield return new WaitWhile(() =>
                !chatManager.GetIsClear());
            done = true;
        }

        #endregion

        #region Out

        public override NpcBattleAfterEffect GetInstantiatedEffect()
        {
            NpcBattleAfterEffect effect = Instantiate(this);
            effect.IsInstantiated();
            return effect;
        }

        #endregion
    }
}