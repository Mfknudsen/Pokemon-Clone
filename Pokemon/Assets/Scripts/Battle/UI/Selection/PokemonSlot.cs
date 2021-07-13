#region SDK

using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
{
    public class PokemonSlot : MonoBehaviour
    {
        private PokemonSelection pokemonSelection;
        private Pokemon pokemon;

        public void SetPokemon(PokemonSelection pokemonSelection, Pokemon pokemon)
        {
            this.pokemonSelection = pokemonSelection;
            this.pokemon = pokemon;
        }

        public void Trigger()
        {
            pokemonSelection.SendPokemon(pokemon);
        }
    }
}