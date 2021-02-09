using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BattleUI
{
    public class PokemonDisplay : MonoBehaviour
    {
        #region Values
        public Pokemon pokemon = null;

        public TextMeshProUGUI nameDisplay = null, healthDisplay = null;
        public ProgressBar healthBar = null;
        #endregion

        private void Update()
        {
            if (pokemon != null)
            {
                float healthToDisplay = pokemon.GetCurrentHealth();

                if (healthToDisplay < 1 && healthToDisplay > 0)
                    healthToDisplay = 1;

                healthDisplay.text = pokemon.GetStat(Stat.HP) + " / " + (int)healthToDisplay;
                healthBar.SetCurrentBar(healthToDisplay);
            }
        }

        #region Setters
        public void SetNewPokemon(Pokemon newPokemon)
        {
            if (newPokemon != null)
            {
                pokemon = newPokemon;
                nameDisplay.text = pokemon.GetName() + " " + pokemon.GetLevel() + "Lv";
                healthBar.SetBarMax(pokemon.GetStat(Stat.HP));
            }
        }
        #endregion
    }
}
