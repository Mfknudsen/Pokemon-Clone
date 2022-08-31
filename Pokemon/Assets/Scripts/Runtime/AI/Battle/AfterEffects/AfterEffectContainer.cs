#region Packages

using System;
using System.Linq;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle.AfterEffects
{
    [Serializable]
    public class AfterEffectContainer
    {
        [SerializeField] private NpcBattleAfterEffect[] effects;

        #region In

        public void OnValidate()
        {
            for (int i = 0; i < effects.Length; i++)
            {
                NpcBattleAfterEffect effect = effects[i];
                effects[i] = effect.GetIsInstantiated() ? effect : effect.GetInstantiatedEffect();
            }
        }

        public void SetInput<T>(object i) where T : NpcBattleAfterEffect
        {
            foreach (NpcBattleAfterEffect effect in effects.Where(e => e is T))
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
            return effects.Any(e => !e.Done());
        }

        #endregion
    }
}