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
        public RoundDoneState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            OperationManager operationManager = OperationManager.instance;

            #region End Turn Abilities

            AbilityOversight abilityOversight = this.manager.GetAbilityOversight();
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

            if (this.manager.CheckTeamDefeated(true))
                this.manager.SetState(new LostState(this.manager));
            else if (this.manager.CheckTeamDefeated(false))
                this.manager.SetState(new WinState(this.manager));
            else
            {
                SpotOversight spotOversight = this.manager.GetSpotOversight();

                spotOversight.Reorganise(true);

                this.manager.SetState(new PlayerTurnState(this.manager));
            }

            #endregion
        }
    }
}