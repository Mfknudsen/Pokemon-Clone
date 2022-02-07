#region Packages

using System;
using System.Linq;
using Mfknudsen.AI.Battle.AfterEffects;
using Mfknudsen.Battle.Systems;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Battle.BeforeEffects
{
    [Serializable]
    public class BeforeEffectContainer
    {
        #region Values

        [SerializeField] private NpcBattleBeforeEffect[] effects;

        #endregion

        #region In

        public void OnValidate()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                NpcBattleBeforeEffect effect = effects[i];
                effects[i] = effect.GetIsInstantiated() ? effect : effect.GetInstantiatedEffect();
            }
        }

        public void SetInput<T>(object i) where T : NpcBattleBeforeEffect
        {
            foreach (NpcBattleBeforeEffect effect in effects.Where(e => e is T))
                effect.SetInput(i);
        }

        public void Trigger()
        {
            // ReSharper disable once CoVariantArrayConversion
            OperationManager.instance.AddOperationsContainer(new OperationsContainer(effects));
        }

        #endregion

        #region Out

        public bool AllDone()
        {
            return !effects.Any(e => !e.Done());
        }

        #endregion
    }
}