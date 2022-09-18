#region Packages

using System;
using System.Linq;
using Runtime.Systems.Operation;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle.BeforeEffects
{
    [Serializable]
    public class BeforeEffectContainer
    {
        #region Values

        [SerializeField, Required] private OperationManager operationManager; 
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
            operationManager.AddOperationsContainer(new OperationsContainer(effects));
        }

        #endregion

        #region Out

        public bool AllDone()
        {
            return !effects.Any(e => !e.IsOperationDone());
        }

        #endregion
    }
}