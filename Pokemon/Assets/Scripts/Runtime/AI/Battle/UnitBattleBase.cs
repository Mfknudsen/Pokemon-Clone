#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.AI.Battle.AfterEffects;
using Runtime.AI.Battle.BeforeEffects;
using Runtime.Battle.Systems;
using Runtime.Communication;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle
{
    [RequireComponent(typeof(BattleStarter))]
    public class UnitBattleBase : UnitBase
    {
        #region Values

        [SerializeField, Required] private ChatManager chatManager;

        [FoldoutGroup("Before Battle")] [SerializeField]
        private BattleStarter battleStarter;

        [FoldoutGroup("Before Battle")] [SerializeField]
        private BeforeEffectContainer[] beforeEffects = Array.Empty<BeforeEffectContainer>();

        [FoldoutGroup("Before Battle")] [SerializeField]
        private AfterEffectContainer[] afterEffects = Array.Empty<AfterEffectContainer>();

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
                container.SetInput<TriggerBeforeChat>(chatInput);

            foreach (AfterEffectContainer container in afterEffects)
                container.SetInput<TriggerAfterChat>(chatInput);

            battleStarter ??= GetComponent<BattleStarter>();
        }

        #endregion

        #region In

        public override void Trigger()
        {
            if (battleStarter.GetPlayerWon())
                chatManager.Add(idleChat);
            else
                StartCoroutine(BeforeBattle());
        }

        #endregion

        #region Internal

        #region Setup Input

        private Dictionary<string, string> SetupChatInput()
        {
            if (chatKeys == null)
                return null;

            Dictionary<string, string> chatInput = new();
            for (int i = 0; i < chatKeys.Length && i < chatValues.Length; i++)
                chatInput.Add(chatKeys[i], chatValues[i]);

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