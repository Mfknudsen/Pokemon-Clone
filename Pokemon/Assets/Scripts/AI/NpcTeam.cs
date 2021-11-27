#region Packages

using System.Collections.Generic;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

namespace Mfknudsen.NPC
{
    [System.Serializable]
    public class NpcTeam : Team
    {
        [SerializeField] private List<PossiblePokemon> allPossible;

        public void Select(Team toAddTo)
        {
            foreach (PossiblePokemon possiblePokemon in allPossible)
                toAddTo.AddNewPokemonToTeam(possiblePokemon.GetRandomFromList());
        }
    }

    [System.Serializable]
    internal struct PossiblePokemon
    {
        [SerializeField] private List<Pokemon> pokemons;

        public Pokemon GetRandomFromList()
        {
            return pokemons[Random.Range(0, pokemons.Count - 1)];
        }
    }
}