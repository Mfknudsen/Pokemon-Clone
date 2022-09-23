#region Packages

using System.Collections;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;
using UnityEngine;

#endregion

// ReSharper disable SuspiciousTypeConversion.Global
namespace Runtime.Battle.Systems.States
{
    public class RoundDoneState : State
    {
        public RoundDoneState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager, UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager, uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            #region End Turn Abilities

            AbilityOversight abilityOversight = this.battleManager.GetAbilityOversight();
            foreach (IOnTurnEnd onTurnEnd in abilityOversight.ListOfSpecific<IOnTurnEnd>())
            {
                if (onTurnEnd is not IOperation iOperation) continue;

                OperationsContainer container = new();
                container.Add(iOperation);
                this.operationManager.AddOperationsContainer(container);

                yield return new WaitWhile(() => !this.operationManager.GetDone());
            }

            #endregion

            #region End Battle

            if (this.battleManager.CheckTeamDefeated(true))
                this.battleManager.SetState(new LostState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
            else if (this.battleManager.CheckTeamDefeated(false))
                this.battleManager.SetState(new WinState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
            else
            {
                SpotOversight spotOversight = this.battleManager.GetSpotOversight();

                spotOversight.Reorganise(true);

                this.battleManager.SetState(new PlayerTurnState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
            }

            #endregion
        }
    }
}