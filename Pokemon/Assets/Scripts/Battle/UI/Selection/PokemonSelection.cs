#region SDK

using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using TMPro;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.UI.Selection
{
    public class PokemonSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private GameObject background;
        [SerializeField] private PokemonSlot[] pokemonSlots;
        private Team playerTeam;

        #endregion

        #region In

        public void Setup()
        {
            playerTeam = MasterPlayer.instance.GetTeam();
        }

        public void DisplaySelection()
        {
            background.SetActive(true);

            for (int i = 0; i < 6; i++)
            {
                Pokemon pokemon = playerTeam.GetPokemonByIndex(i);

                pokemonSlots[i].SetPokemon(this, pokemon);
            }
        }

        public void DisableDisplaySelection()
        {
            background.SetActive(false);
        }

        public void SendPokemon(Pokemon pokemon)
        {
        }

        #endregion
    }
}