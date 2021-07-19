#region SDK

using System.Collections.Generic;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

// ReSharper disable IdentifierTypo
namespace Mfknudsen.Trainer
{
    public class Team : MonoBehaviour
    {
        #region Values

        [SerializeField] private List<Pokemon> pokemons = new List<Pokemon>(6);
        private bool ready;

        private void OnValidate()
        {
            if (pokemons.Count != 6)
            {
                List<Pokemon> tempList = new List<Pokemon>();

                foreach (Pokemon p in pokemons)
                {
                    if (p != null && tempList.Count != 6)
                        tempList.Add(p);
                    else if (tempList.Count == 6)
                        break;
                }

                for (int i = 0; i < tempList.Count; i++)
                    pokemons[i] = tempList[i];
            }

            for (int i = 0; i < pokemons.Count; i++)
            {
                if (pokemons[i] != null)
                {
                    if (!pokemons[i].GetIsInstantiated())
                        pokemons[i] = pokemons[i].GetPokemon();
                }
            }
        }

        #endregion

        #region Getters

        public bool GetReady()
        {
            return ready;
        }

        public bool HasMorePokemon()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Pokemon pokemon in pokemons)
            {
                if (pokemon is null) continue;

                Condition c = pokemon.GetConditionOversight().GetNonVolatileStatus();

                if (c is FaintedCondition) continue;

                return true;
            }

            return false;
        }

        public bool CanSendMorePokemon()
        {
            foreach (Pokemon pokemon in pokemons)
            {
                if (pokemon is null) continue;

                if (pokemon.GetInBattle() || pokemon.GetGettingSwitched()) continue;

                if (pokemon.GetConditionOversight().GetNonVolatileStatus() is null)
                    return true;

                if (!(pokemon.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition))
                    return true;
            }

            return false;
        }

        public bool PartOfTeam(Pokemon pokemon)
        {
            foreach (Pokemon p in pokemons)
            {
                if (p == pokemon)
                    return true;
            }

            return false;
        }

        public Pokemon GetFirstOut()
        {
            foreach (Pokemon pokemon in pokemons)
            {
                if (pokemon is null) continue;

                if (!pokemon.GetInBattle() && !pokemon.GetGettingSwitched())
                    return pokemon;
            }

            return null;
        }

        #endregion

        #region In

        public void Setup()
        {
            for (int i = 0; i < pokemons.Count; i++)
            {
                if (pokemons[i] != null)
                    pokemons[i] = pokemons[i].GetPokemon();
            }

            ready = true;
        }

        public Pokemon GetPokemonByIndex(int index)
        {
            if (index < 0 || index > 5 || pokemons[index] is null) return null;

            return pokemons[index].GetPokemon();
        }

        public void SwitchTeamPlaces(Pokemon from, Pokemon to)
        {
            int fromIndex = pokemons.IndexOf(from), toIndex = pokemons.IndexOf(to);

            Pokemon toStore = pokemons[toIndex];
            pokemons[toIndex] = pokemons[fromIndex];
            pokemons[fromIndex] = toStore;
        }

        #endregion
    }
}