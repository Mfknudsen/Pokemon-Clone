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
                nameDisplay.text = pokemon.GetName() + " " + pokemon.GetLevel() + "Lv";
                healthBar.SetBarMax(pokemon.GetHealth());
            }
        }

        private void Update()
        {
            if (pokemon != null) {
                healthDisplay.text = pokemon.GetHealth() + " / " + pokemon.GetCurrentHealth();
                healthBar.SetCurrentBar(pokemon.GetCurrentHealth());
            }
        }
    }
}
