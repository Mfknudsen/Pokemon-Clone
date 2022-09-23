#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Spots;
using Runtime.Battle.UI.Selection;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;
using Runtime.Trainer;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class PlayerTurnState : State
    {
        public PlayerTurnState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager, UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager, uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            Cursor.visible = true;
            Team playerTeam = this.playerManager.GetTeam();
            SpotOversight spotOversight = this.battleManager.GetSpotOversight();

            foreach (Pokemon pokemon in spotOversight.GetSpots()
                         .Select(spot => spot.GetActivePokemon())
                         .Where(pokemon => pokemon is not null && playerTeam.PartOfTeam(pokemon)))
            {
                this.battleManager.GetSelectionMenu().DisplaySelection(SelectorGoal.Turn, pokemon);

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            this.battleManager.GetSelectionMenu().DisableDisplaySelection();

            this.battleManager.SetState(new ComputerTurnState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
        }
    }
}