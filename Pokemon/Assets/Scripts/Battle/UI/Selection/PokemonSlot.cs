#region SDK

using Mfknudsen.Pokémon;
using TMPro;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
{
    public class PokemonSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gui;
        
        private PokemonSelection pokemonSelection;
        private Pokemon pokemon;

        public void SetPokemon(PokemonSelection pokemonSelection, Pokemon pokemon)
        {
            this.pokemonSelection = pokemonSelection;
            this.pokemon = pokemon;

            gui.text = pokemon is null ? "" : pokemon.GetName();
        }

        public void Trigger()
        {
            pokemonSelection.SendPokemon(pokemon);
        }
    }
}