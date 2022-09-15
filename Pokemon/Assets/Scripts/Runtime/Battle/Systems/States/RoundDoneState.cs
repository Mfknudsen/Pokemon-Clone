#region Packages

using System.Collections;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Battle.Systems.Spots;
using Runtime.Systems;
using UnityEngine;

#endregion

// ReSharper disable SuspiciousTypeConversion.Global
namespace Runtime.Battle.Systems.States
{
    public class RoundDoneState : State
    {
        public RoundDoneState(BattleManager battleManager) : base(battleManager)
        {
        }

        public override IEnumerator Tick()
        {
            OperationManager operationManager = OperationManager.instance;

            #region End Turn Abilities

            AbilityOversight abilityOversight = this.battleManager.GetAbilityOversight();
            foreach (IOnTurnEnd onTurnEnd in abilityOversight.ListOfSpecific<IOnTurnEnd>())
            {
                if (onTurnEnd is not IOperation iOperation) continue;

                OperationsContainer container = new();
                container.Add(iOperation);
                operationManager.AddOperationsContainer(container);

                yield return new WaitWhile(() => !operationManager.GetDone());
            }

            #endregion

            #region End Battle

            if (this.battleManager.CheckTeamDefeated(true))
                this.battleManager.SetState(new LostState(this.battleManager));
            else if (this.battleManager.CheckTeamDefeated(false))
                this.battleManager.SetState(new WinState(this.battleManager));
            else
            {
                SpotOversight spotOversight = this.battleManager.GetSpotOversight();

                spotOversight.Reorganise(true);

                this.battleManager.SetState(new PlayerTurnState(this.battleManager));
            }

            #endregion
        }
    }
}