#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Battle.Systems.Spots;
using UnityEngine;

#endregion

// ReSharper disable SuspiciousTypeConversion.Global
namespace Mfknudsen.Battle.Systems.States
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

            AbilityOversight abilityOversight = manager.GetAbilityOversight();
            foreach (IOnTurnEnd onTurnEnd in abilityOversight.ListOfSpecific<IOnTurnEnd>())
            {
                if (!(onTurnEnd is IOperation iOperation)) continue;

                OperationsContainer container = new OperationsContainer();
                container.Add(iOperation);
                operationManager.AddOperationsContainer(container);

                yield return new WaitWhile(() => !operationManager.GetDone());
            }

            #endregion

            #region End Battle
            
            if (manager.CheckTeamDefeated(true))
                manager.SetState(new LostState(manager));
            else if (manager.CheckTeamDefeated(false))
                manager.SetState(new WinState(manager));
            else
            {
                SpotOversight spotOversight = manager.GetSpotOversight();

                spotOversight.Reorganise(true);

                manager.SetState(new PlayerTurnState(manager));
            }

            #endregion
        }
    }
}