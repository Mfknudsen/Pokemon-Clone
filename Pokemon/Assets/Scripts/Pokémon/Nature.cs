#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon
{
    [CreateAssetMenu(menuName = "Pokemon/Nature")]
    public class Nature : ScriptableObject
    {
        [SerializeField] private string natureName;

        // ReSharper disable once IdentifierTypo
        [SerializeField] private Stat positiveStat, negativeStat;
        [SerializeField] private int positiveBonus, negativeReduction;

        public float GetNatureStat(Stat stat)
        {
            if (stat == positiveStat)
                return positiveBonus;

            return stat == negativeStat ? negativeReduction : 1;
        }
    }
}