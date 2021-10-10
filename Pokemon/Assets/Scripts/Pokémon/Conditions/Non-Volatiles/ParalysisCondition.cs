#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Conditions.Non_Volatiles
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Paralysis", order = 1)]
    public class ParalysisCondition : NonVolatileCondition, IOperation
    {
        [SerializeField] private Chat onEffectChat = null;
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