#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Conditions.Non_Volatiles
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Sleep", order = 1)]
    public class SleepCondition : NonVolatileCondition, IOperation
    {
        [SerializeField] private Chat onEffectChat;
        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Done()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Operation()
        {
            throw new System.NotImplementedException();
        }

        public void End()
        {
        }
    }
}