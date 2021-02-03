#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace Trainer
{
    public class Team : MonoBehaviour
    {
        [SerializeField] private Pokemon[] pokemons = new Pokemon[6];
        private bool ready = false;

        public void Setup()
        {
            for (int i = 0; i < pokemons.Length; i++)
            {
                if (pokemons[i] != null)
                    pokemons[i] = pokemons[i].GetPokemon();
            }

            ready = true;
        }

        public bool GetReady()
        {
            return ready;
        }

        public Pokemon GetPokemonByIndex(int index)
        {
            if (index >= 0 && index <= 5)
            {
                if (pokemons[index] != null)
                    return pokemons[index].GetPokemon();
            }

            return null;
        }

        #region SwitchingPokemon
        public void SwitchTeamPlaces(int from, int to)
        {
            Pokemon toStore = pokemons[to];
            pokemons[to] = pokemons[from];
            pokemons[from] = toStore;
        }

        public void SwitchTeamPlaces(Pokemon from, Pokemon to)
        {
            int i = 0, j = 1;

            for (i = 0; i < pokemons.Length; i++)
            {
                if (pokemons[i] != null)
                {
                    if (pokemons[i] == from)
                        break;
                }
            }

            for (j = 0; j < pokemons.Length; j++)
            {
                if (pokemons[j] != null)
                {
                    if (pokemons[j] == to)
                        break;
                }
            }

            Pokemon toStore = pokemons[j];
            pokemons[j] = pokemons[i];
            pokemons[i] = toStore;
        }
        #endregion

        public bool HasMorePokemon()
        {
            foreach (Pokemon pokemon in pokemons)
            {
                if (pokemon != null)
                {
                    Condition c = pokemon.GetConditionOversight().GetNonVolatileStatus();
                    if (c != null)
                    {
                        if (c.GetConditionName() != NonVolatile.Fainted.ToString())
                            return true;
                    }
                    else
                        return true;
                }
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
    }
}