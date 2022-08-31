#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Spots;
using Runtime.Battle.UI.Selection;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Trainer;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class PlayerTurnState : State
    {
        public PlayerTurnState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            Cursor.visible = true;
            Team playerTeam = PlayerManager.instance.GetTeam();
            SpotOversight spotOversight = this.manager.GetSpotOversight();

            foreach (Pokemon pokemon in spotOversight.GetSpots()
                         .Select(spot => spot.GetActivePokemon())
                         .Where(pokemon => pokemon is not null && playerTeam.PartOfTeam(pokemon)))
            {
                this.manager.GetSelectionMenu().DisplaySelection(SelectorGoal.Turn, pokemon);

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            this.manager.GetSelectionMenu().DisableDisplaySelection();

            this.manager.SetState(new ComputerTurnState(this.manager));
        }
    }
}