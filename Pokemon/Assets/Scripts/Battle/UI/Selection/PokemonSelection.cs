#region SDK

using Mfknudsen.Battle.Actions;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
{
    public class PokemonSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private GameObject background;
        [SerializeField] private PokemonSlot[] pokemonSlots;
        private Team playerTeam;
        private SwitchAction switchAction;

        #endregion

        #region In

        public void Setup()
        {
            playerTeam = PlayerManager.Instance.GetTeam();
        }

        public void DisplaySelection(SwitchAction switchAction)
        {
            this.switchAction = switchAction;

            background.SetActive(true);

            for (int i = 0; i < 6; i++)
                pokemonSlots[i].SetPokemon(this, playerTeam.GetPokemonByIndex(i));
        }

        public void DisableDisplaySelection()
        {
            background.SetActive(false);
        }

        public void SendPokemon(Pokemon pokemon)
        {
            if (pokemon is null) return;

            switchAction.SetNextPokemon(pokemon);

            pokemon.SetGettingSwitched(true);

            switchAction = null;
        }

        #endregion
    }
}