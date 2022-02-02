#region Packages

using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
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
            playerTeam = PlayerManager.instance.GetTeam();
        }

        public void DisplaySelection(SwitchAction switchAction)
        {
            this.switchAction = switchAction;

            background.SetActive(true);

            for (int i = 0; i < 6; i++)
                pokemonSlots[i].SetPokemon(
                    this,
                    playerTeam.GetPokemonByIndex(i));
        }

        public void DisableDisplaySelection()
        {
            background.SetActive(false);
        }

        public void SendPokemon(Pokemon pokemon)
        {
            if (pokemon is null) return;

            switchAction.SetNextPokemon(
                pokemon);

            switchAction.SetSpot(
                BattleManager.instance.GetSpotOversight().GetSpots()
                    .FirstOrDefault(s => s.GetActivePokemon() == switchAction.GetCurrentPokemon()));

            switchAction.SetTeam(
                PlayerManager.instance.GetTeam());

            pokemon.SetGettingSwitched(
                true);

            switchAction.GetCurrentPokemon().SetBattleAction(
                switchAction);

            switchAction = null;
        }

        #endregion
    }
}