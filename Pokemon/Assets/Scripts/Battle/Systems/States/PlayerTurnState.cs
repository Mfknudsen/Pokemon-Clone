#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using Mfknudsen.Player;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
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