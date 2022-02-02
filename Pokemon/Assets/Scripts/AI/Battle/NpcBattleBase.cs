#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI.Battle.AfterEffects;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Battle
{
    public class NpcBattleBase : NpcBase
    {
        #region Value

        [FoldoutGroup("Before Battle")] [SerializeField]
        private BattleStarter battleStarter;

        [FoldoutGroup("Before Battle")] [SerializeField]
        private BeforeEffectContainer[] beforeEffects;

        [FoldoutGroup("Before Battle")] [SerializeField]
        private AfterEffectContainer[] afterEffects;

        [FoldoutGroup("Input")] [SerializeField]
        private string[] chatKeys, chatValues;
        
        #endregion

        #region Build In States

        private void Start()
        {
            //Setup Input
            Dictionary<string, string> chatInput = new Dictionary<string, string>();

            //Apply
            foreach (BeforeEffectContainer container in beforeEffects)
            {
            }

            foreach (AfterEffectContainer container in afterEffects)
            {
                container.SetInput<TriggerChat>(chatInput);
            }
        }

        #endregion

        #region In

        public override void Trigger()
        {
            if (battleStarter == null) return;

            if (!battleStarter.GetPlayerWon())
            {
                StartCoroutine(BeforeBattle());
            }
            else
                ChatManager.instance.Add(idleChat);
        }

        #endregion

        #region Internal

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

        #endregion
    }

    [Serializable]
    internal class BeforeEffectContainer
    {
        [SerializeField] private NpcBattleBeforeEffect[] effects;

        public void SetInput<T>(object i) where T : NpcBattleBeforeEffect
        {
            foreach (NpcBattleBeforeEffect effect in effects.Where(e => e is T))
                effect.SetInput(i);
        }

        public bool AllDone()
        {
            return effects.Any(e => !e.Done());
        }

        public void Trigger()
        {
            // ReSharper disable once CoVariantArrayConversion
            OperationManager.instance.AddOperationsContainer(new OperationsContainer(effects));
        }
    }

    [Serializable]
    internal class AfterEffectContainer
    {
        [SerializeField] private NpcBattleAfterEffect[] effects;

        public void SetInput<T>(object i) where T : NpcBattleAfterEffect
        {
            foreach (NpcBattleAfterEffect effect in effects.Where(e => e is T))
                effect.SetInput(i);
        }

        public bool AllDone()
        {
            return effects.Any(e => !e.Done());
        }

        public void Trigger()
        {
            // ReSharper disable once CoVariantArrayConversion
            OperationManager.instance.AddOperationsContainer(new OperationsContainer(effects));
        }
    }
}