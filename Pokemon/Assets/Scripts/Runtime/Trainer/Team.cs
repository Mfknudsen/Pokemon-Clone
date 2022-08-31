#region Packages

using System.Collections.Generic;
using System.Linq;
using Runtime.Pokémon;
using Runtime.Pokémon.Conditions.Non_Volatiles;
using UnityEngine;

#endregion

namespace Runtime.Trainer
{
    public class Team : MonoBehaviour
    {
        #region Values

        [SerializeField] private List<Pokemon> pokemons = new(6);
        private readonly List<BoxContainer> boxContainers = new();

        private void OnValidate()
        {
            if (pokemons.Count == 6) return;
            
            List<Pokemon> tempList = new();

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

        #endregion

        #region Getters

        public int GetTeamCount()
        {
            return pokemons.Count(p => p != null);
        }

        public bool HasMorePokemon()
        {
            return pokemons.FirstOrDefault(p => !(p.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition));
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
                Pokemon pokemon = pokemons[i];
                if (pokemon == null || pokemon.GetIsInstantiated()) continue;

                pokemon = Instantiate(pokemon);

                pokemon.SetIsInstantiated(true);

                pokemons[i] = pokemon;
            }
        }

        public Pokemon GetPokemonByIndex(int index)
        {
            if (index < 0 ||
                index > 5 ||
                index >= pokemons.Count) return null;
            
            return pokemons[index];
        }

        public void SwitchTeamPlaces(Pokemon from, Pokemon to)
        {
            int fromIndex = pokemons.IndexOf(from), toIndex = pokemons.IndexOf(to);

            (pokemons[toIndex], pokemons[fromIndex]) = (pokemons[fromIndex], pokemons[toIndex]);
        }

        public void AddNewPokemonToTeam(Pokemon toAdd)
        {
            for (int i = 0; i < pokemons.Capacity; i++)
            {
                if (pokemons[i] != null) continue;

                pokemons[i] = toAdd;
                return;
            }

            if (boxContainers.Any(container => container.AddPokemon(toAdd)))
                return;

            BoxContainer c = new();
            c.AddPokemon(toAdd);
            boxContainers.Add(c);
        }

        public void RemovePokemonFromTeam(Pokemon toRemove)
        {
            for (int i = 0; i < pokemons.Capacity; i++)
            {
                if (pokemons[i] != toRemove) continue;

                pokemons[i] = null;
                return;
            }

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            boxContainers.Any(boxContainer => boxContainer.RemovePokemon(toRemove));
        }

        #endregion
    }

    internal class BoxContainer
    {
        private readonly List<Pokemon> pokemons = new(48);

        public bool AddPokemon(Pokemon toAdd)
        {
            for (int i = 0; i < pokemons.Capacity; i++)
            {
                if (pokemons[i] != null) continue;

                pokemons[i] = toAdd;
                return true;
            }

            return false;
        }

        public bool RemovePokemon(Pokemon toRemove)
        {
            for (int i = 0; i < pokemons.Capacity; i++)
            {
                if (pokemons[i] != toRemove) continue;

                pokemons[i] = toRemove;
                return true;
            }

            return false;
        }
    }
}