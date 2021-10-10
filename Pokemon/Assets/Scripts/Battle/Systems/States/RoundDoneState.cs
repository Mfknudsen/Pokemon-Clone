#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Player;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

// ReSharper disable SuspiciousTypeConversion.Global
namespace Mfknudsen.Battle.Systems.States
{
    public class RoundDoneState : State
    {
        private readonly Team playerTeam;

        public RoundDoneState(BattleManager manager) : base(manager)
        {
            playerTeam = PlayerManager.instance.GetTeam();
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

                while (!operationManager.GetDone())
                    yield return null;
            }

            #endregion

            #region End Battle

            SpotOversight spotOversight = manager.GetSpotOversight();

            bool endBattle = true;

            spotOversight.Reorganise(true);

            if (spotOversight.GetSpots().Count > 1)
            {
                Spot spotA = spotOversight.GetSpots()[0];

                for (int i = 1; i < spotOversight.GetSpots().Count; i++)
                {
                    Spot spotB = spotOversight.GetSpots()[i];

                    if (spotA.GetTeamNumber() == spotB.GetTeamNumber()) continue;

                    endBattle = false;

                    break;
                }
            }

            if (endBattle)
            {
                bool playerVictory = playerTeam.HasMorePokemon();
                Debug.Log("Victory: " + playerVictory);
                if (playerVictory)
                    manager.SetState(new WinState(manager));
                else
                    manager.SetState(new LostState(manager));
            }
            else
                manager.SetState(new PlayerTurnState(manager));

            #endregion
        }
    }
}