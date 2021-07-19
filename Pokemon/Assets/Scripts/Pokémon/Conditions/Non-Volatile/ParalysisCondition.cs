#region SDK

using System.Collections;
using Mfknudsen.Comunication;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Paralysis", order = 1)]
    public class ParalysisCondition : Condition, INonVolatile
    {
        [SerializeField] private Chat onEffectChat = null;
        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator ActivateCondition(ConditionOversight activator)
        {
            throw new System.NotImplementedException();
        }
    }
}