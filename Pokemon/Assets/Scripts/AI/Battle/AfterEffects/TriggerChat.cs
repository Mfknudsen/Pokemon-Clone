#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Communication;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Battle.AfterEffects
{
    [Serializable]
    public class TriggerChat : NpcBattleAfterEffect
    {
        [SerializeField] private Chat chat;

        private Dictionary<string, string> input;

        public override void SetInput(object i)
        {
            try
            {
                input = i as Dictionary<string, string>;
            }
            catch (Exception e)
            {
                Debug.LogError("Input was not Dictionary of string/string - \n"+e.Message);
                input = null;
            }
        }

        public override IEnumerator Operation()
        {
            if (input == null) yield break;

            // ReSharper disable once AccessToStaticMemberViaDerivedType
            chat = ScriptableObject.Instantiate(chat);
            foreach (string inputKey in input.Keys)
                chat.AddToOverride(
                    inputKey,
                    input[inputKey]);

            ChatManager.instance.Add(chat);

            yield return new WaitWhile(() =>
                !ChatManager.instance.GetIsClear());
            done = true;
        }
    }
}