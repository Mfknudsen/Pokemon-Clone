#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
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
        private readonly List<BoxContainer> boxContainers = new List<BoxContainer>();

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
        }

        #endregion

        #region Getters

        public bool GetReady()
        {
            return ready;
        }

        public int GetTeamCount()
        {
            return pokemons.Count(p => p != null);
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
                Pokemon pokemon = pokemons[i];
                if (pokemon == null || pokemon.GetIsInstantiated()) continue;

                pokemon = Instantiate(pokemon);

                pokemon.SetIsInstantiated(true);

                pokemons[i] = pokemon;
            }

            ready = true;
        }

        public Pokemon GetPokemonByIndex(int index)
        {
            if (index < 0 || 
                index > 5 || 
                index <= pokemons.Count 
                || pokemons[index] == null) return null;

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

            BoxContainer c = new BoxContainer();
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
        private readonly List<Pokemon> pokemons = new List<Pokemon>(48);

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