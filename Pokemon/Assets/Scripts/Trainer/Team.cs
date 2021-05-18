#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Monster;
using Monster.Conditions;
#endregion

namespace Trainer
{
    public class Team : MonoBehaviour
    {
        #region Values
        [SerializeField] private bool CLEARLIST = false;
        [Space]
        [SerializeField] private Pokemon[] pokemons = new Pokemon[6];
        private bool ready = false;

        private void OnValidate()
        {
            if (pokemons.Length != 6)
            {
                List<Pokemon> tempList = new List<Pokemon>();

                foreach (Pokemon p in pokemons)
                {
                    if (p != null && tempList.Count != 6)
                        tempList.Add(p);
                    else if (tempList.Count == 6)
                        break;
                }

                pokemons = new Pokemon[6];

                for (int i = 0; i < tempList.Count; i++)
                    pokemons[i] = tempList[i];
            }

            for (int i = 0; i < pokemons.Length; i++)
            {
                if (pokemons[i] != null)
                {
                    if (!pokemons[i].GetIsInstantiated())
                        pokemons[i] = pokemons[i].GetPokemon();
                }
            }

            if (CLEARLIST)
            {
                foreach (Pokemon p in pokemons)
                {
                    if (p != null)
                        Destroy(p);
                }

                pokemons = new Pokemon[6];

                CLEARLIST = false;
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

        public bool CanSendMorePokemon()
        {
            foreach (Pokemon pokemon in pokemons)
            {
                if (pokemon != null)
                {
                    if (!pokemon.GetInBattle() && !pokemon.GetGettingSwitched())
                    {
                        if (pokemon.GetConditionOversight().GetNonVolatileStatus() == null)
                            return true;
                        else if (pokemon.GetConditionOversight().GetNonVolatileStatus().GetConditionName() != NonVolatile.Fainted.ToString())
                            return true;
                    }
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

        public Pokemon GetFirstOut()
        {
            foreach (Pokemon p in pokemons)
            {
                if (p != null)
                {
                    if (!p.GetInBattle() && !p.GetGettingSwitched())
                        return p;
                }
            }

            return null;
        }
        #endregion

        #region In
        public void Setup()
        {
            for (int i = 0; i < pokemons.Length; i++)
            {
                if (pokemons[i] != null)
                    pokemons[i] = pokemons[i].GetPokemon();
            }

            ready = true;
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
        #endregion
    }
}