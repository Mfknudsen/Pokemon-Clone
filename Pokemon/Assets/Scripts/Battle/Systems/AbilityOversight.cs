#region SDK

using System.Collections.Generic;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class AbilityOversight
    {
        private List<Ability> abilities;

        #region Getters

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public List<Ability> GetAbilities()
        {
            return abilities;
        }

        #endregion
        
        #region In

        public void Setup()
        {
            abilities = new List<Ability>();
        }

        public void AddAbility(Ability ability)
        {
            if (!abilities.Contains(ability) && !(ability is null))
                abilities.Add(ability);
        }

        public void RemoveAbility(Ability ability)
        {
            if (abilities.Contains(ability))
                abilities.Remove(ability);
        }

        #endregion

        #region Out

        public List<T> ListOfSpecific<T>()
        {
            List<T> result = new List<T>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Ability ability in abilities)
            {
                if (ability is T tAbility)
                    result.Add(tAbility);
            }

            return result;
        }

        #endregion
    }
}