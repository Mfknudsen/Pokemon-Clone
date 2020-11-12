using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trainer
{
    public class Team : MonoBehaviour
    {
        public Pokemon[] pokemons = new Pokemon[6];

        public void Setup()
        {
            for (int i = 0; i < pokemons.Length; i++)
            {
                if (pokemons[i] != null)
                {
                    pokemons[i] = Instantiate(pokemons[i]);
                }
            }
        }

        public void SwitchTeamPlaces(int from, int to)
        {
            Pokemon toStore = pokemons[to];
            pokemons[to] = pokemons[from];
            pokemons[from] = toStore;
        }

        public Pokemon GetPokemonByIndex(int index)
        {
            if (index >= 0 && index <= 5) {
                if (pokemons[index] != null)
                    return pokemons[index];
            }

            return null;
        }
    }
}