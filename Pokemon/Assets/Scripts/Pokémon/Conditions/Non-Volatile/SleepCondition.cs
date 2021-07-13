#region SDK

using Mfknudsen.Comunication;
using UnityEngine;

//Custom

#endregion

namespace Mfknudsen.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Sleep", order = 1)]
    public class SleepCondition : Condition, INonVolatile
    {
        [SerializeField] private Chat onEffectChat = null;
    }
}