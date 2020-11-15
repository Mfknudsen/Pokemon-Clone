﻿using System.Collections;
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
            if (pokemon != null)
            {
                float healthToDisplay = pokemon.GetCurrentHealth();

                if (healthToDisplay < 1 && healthToDisplay > 0)
                    healthToDisplay = 1;

                healthDisplay.text = pokemon.GetHealth() + " / " + (int)healthToDisplay;
                healthBar.SetCurrentBar(healthToDisplay);
            }
        }
    }
}
