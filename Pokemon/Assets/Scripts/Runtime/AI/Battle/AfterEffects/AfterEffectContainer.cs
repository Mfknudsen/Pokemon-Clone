#region Packages

using System;
using System.Linq;
using Runtime.Systems.Operation;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle.AfterEffects
{
    [Serializable]
    public class AfterEffectContainer
    {
        [SerializeField, Required] private OperationManager operationManager;
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
            operationManager.AddOperationsContainer(new OperationsContainer(effects));
        }

        #endregion

        #region Out

        public bool AllDone()
        {
            return effects.Any(e => !e.IsOperationDone());
        }

        #endregion
    }
}