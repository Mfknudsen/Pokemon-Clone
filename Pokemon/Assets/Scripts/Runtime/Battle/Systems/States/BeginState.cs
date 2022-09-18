#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;
using UnityEngine;
using Logger = Runtime._Debug.Logger;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class BeginState : State
    {
        private SpotOversight spotOversight;

        public BeginState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager,
            UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager,
            uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            BattleStarter battleStarter = null;

            while (battleStarter == null)
            {
                battleStarter = this.battleManager.GetStarter();
                yield return null;
            }

            this.battleManager.SetSelectionMenu(uiManager.GetSelectionMenu());
            this.battleManager.SetDisplayManager(uiManager.GetDisplayManager());

            #region Setup Spots

            foreach (BattleMember battleMember in battleStarter.GetAllBattleMembers())
            {
                battleMember.Setup();
                battleMember.GetTeam().Setup();
            }

            this.spotOversight = this.battleManager.GetSpotOversight();

            BattleMember[] playerWithAllies = { playerManager.GetBattleMember() };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            playerWithAllies.Concat(battleStarter.GetAllies());
            this.spotOversight.SetupSpots(playerWithAllies, this.battleManager.GetBattlefield().GetAllyField());
            this.spotOversight.SetupSpots(battleStarter.GetEnemies(),
                this.battleManager.GetBattlefield().GetEnemyField());

            this.spotOversight.Reorganise(false);

            #endregion

            this.battleManager.SetupAbilityOversight();

            #region Start Log

            string playersMsg = "Starting Battle Between:";
            string alliesMsg = " - " + playerManager.GetBattleMember().GetName(), enemiesMsg = "";

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in this.battleManager.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (!battleMember || battleMember == playerManager.GetBattleMember()) continue;

                if (battleMember.GetTeamAffiliation() == playerManager.GetBattleMember().GetTeamAffiliation())
                    alliesMsg += ", " + battleMember.GetName();
                else
                {
                    if (!enemiesMsg.Equals(""))
                        enemiesMsg += ", ";
                    else
                        enemiesMsg += " - ";

                    enemiesMsg += battleMember.GetName();
                }
            }

            playersMsg += "\n" + alliesMsg + "\n" + enemiesMsg;

            yield return new WaitWhile(() => !Logger.instance);

            Logger.AddLog("Begin State", playersMsg);

            #endregion

            while (!chatManager.GetIsClear())
                yield return null;

            #region Start Actions

            List<OperationsContainer> switchInActions = new();
            foreach (Spot spot in this.spotOversight.GetSpots())
            {
                spot.SetTransform();

                if (spot.GetBattleMember() is null)
                    continue;

                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = battleMember.GetTeam().GetFirstOut();

                if (!pokemon) continue;

                SwitchAction action = this.battleManager.InstantiateSwitchAction();

                action.SetNextPokemon(battleMember.GetTeam().GetFirstOut());
                action.SetSpot(spot);

                OperationsContainer container = new();
                container.Add(action);
                switchInActions.Add(container);
            }

            operationManager.AddOperationsContainer(switchInActions.ToArray());

            #endregion

            foreach (IOperation i in switchInActions
                         .SelectMany(switchInAction =>
                             switchInAction.GetInterfaces()))
            {
                while (!i.IsOperationDone())
                    yield return null;
            }


            while (!chatManager.GetIsClear() || !operationManager.GetDone())
                yield return null;

            this.spotOversight.Reorganise(false);

            this.battleManager.SetState(new PlayerTurnState(this.battleManager, operationManager, chatManager, uiManager, playerManager));
        }
    }
}