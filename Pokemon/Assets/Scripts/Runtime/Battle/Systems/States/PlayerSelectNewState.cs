#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems.Spots;
using Runtime.Battle.UI.Selection;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class PlayerSelectNewState : State
    {
        public PlayerSelectNewState(BattleManager battleManager, OperationManager operationManager,
            ChatManager chatManager, UIManager uiManager, PlayerManager playerManager) : base(battleManager,
            operationManager, chatManager, uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            Cursor.visible = true;
            List<SwitchAction> switchActions = new();
            SpotOversight oversight = this.battleManager.GetSpotOversight();
            BattleMember playerTeam = this.playerManager.GetBattleMember();

            if (playerTeam.GetTeam().CanSendMorePokemon())
            {
                foreach (Spot spot in oversight.GetSpots()
                             .Where(s =>
                                 s.GetBattleMember() == playerTeam ||
                                 s.GetActivePokemon() != null))
                {
                    SwitchAction switchAction = this.battleManager.InstantiateSwitchAction();

                    switchAction.SetSpot(spot);

                    this.battleManager.GetSelectionMenu().DisplaySelection(SelectorGoal.Switch, switchAction);

                    yield return new WaitWhile(() => !switchAction.GetNextPokemon() ||
                                                     !this.chatManager.GetIsClear());

                    switchActions.Add(switchAction);
                }
            }

            this.battleManager.GetSelectionMenu().DisableDisplaySelection();

            Cursor.visible = false;

            this.battleManager.SetState(new ComputerSelectNewState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager, switchActions));
        }
    }
}