using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BattleUI
{
    public class PokemonDisplay : MonoBehaviour
    {
        public Pokemon pokemon = null;

        public TextMeshProUGUI nameDisplay = null, healthDisplay = null;
        public ProgressBar healthBar = null;

        public void SetNewPokemon(Pokemon newPokemon)
        {
            if (newPokemon != null)
            {
                pokemon = newPokemon;
                nameDisplay.text = newPokemon.name;
            }
        }

        private void Update()
        {
            healthDisplay.text = pokemon.currentHealth + " / " + pokemon.GetHealth();
            healthBar.SetCurrentBar(pokemon.currentHealth);
        }
    }
}
