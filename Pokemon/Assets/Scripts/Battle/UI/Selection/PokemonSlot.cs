#region Packages

using Mfknudsen.Pokémon;
using TMPro;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.UI.Selection
{
    public class PokemonSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gui;

        private PokemonSelection pokemonSelection;
        private Pokemon pokemon;

        public void SetPokemon(PokemonSelection _pokemonSelection, Pokemon _pokemon)
        {
            pokemonSelection = _pokemonSelection;
            pokemon = _pokemon;

            gui.text = _pokemon is null ? "" : _pokemon.GetName();
        }

        public void Trigger()
        {
            pokemonSelection.SendPokemon(pokemon);
        }
    }
}