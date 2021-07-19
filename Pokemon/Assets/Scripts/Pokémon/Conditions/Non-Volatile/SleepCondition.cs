#region SDK

using System.Collections;
using Mfknudsen.Comunication;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Sleep", order = 1)]
    public class SleepCondition : Condition, INonVolatile
    {
        [SerializeField] private Chat onEffectChat;
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