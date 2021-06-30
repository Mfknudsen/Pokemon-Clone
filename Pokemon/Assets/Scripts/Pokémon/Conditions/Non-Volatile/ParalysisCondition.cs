#region SDK

using Mfknudsen.Comunication;
using UnityEngine;

//Custom

#endregion

namespace Mfknudsen.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Paralysis", order = 1)]
    public class ParalysisCondition : Condition
    {
        [SerializeField] private NonVolatile conditionName = NonVolatile.Poison;
        [SerializeField] private Chat onEffectChat = null;

        public override string GetConditionName()
        {
            return conditionName.ToString();
        }
    }
}