#region Packages

using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Trainer;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Battle.UI.Selection
{
    public class PokemonSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private GameObject background;
        [SerializeField] private PokemonSlot[] pokemonSlots;
        private Team playerTeam;
        private SwitchAction switchAction;

        #endregion

        #region In

        public void Setup()
        {
            this.playerTeam = this.playerManager.GetTeam();
        }

        public void DisplaySelection(SwitchAction switchAction)
        {
            this.switchAction = switchAction;

            this.background.SetActive(true);

            for (int i = 0; i < 6; i++)
                this.pokemonSlots[i].SetPokemon(
                    this, this.playerTeam.GetPokemonByIndex(i));
        }

        public void DisableDisplaySelection()
        {
            this.background.SetActive(false);
        }

        public void SendPokemon(Pokemon pokemon)
        {
            if (pokemon is null) return;

            this.switchAction.SetNextPokemon(
                pokemon);

            this.switchAction.SetSpot(
                BattleManager.instance.GetSpotOversight().GetSpots()
                    .FirstOrDefault(s => s.GetActivePokemon() == this.switchAction.GetCurrentPokemon()));

            this.switchAction.SetTeam(this.playerManager.GetTeam());

            pokemon.SetGettingSwitched(
                true);

            this.switchAction.GetCurrentPokemon().SetBattleAction(this.switchAction);

            this.switchAction = null;
        }

        #endregion
    }
}