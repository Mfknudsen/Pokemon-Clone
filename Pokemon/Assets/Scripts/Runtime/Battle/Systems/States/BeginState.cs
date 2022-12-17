#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Systems;
using Runtime.Systems.UI;
using UnityEngine;
using Logger = Runtime._Debug.Logger;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class BeginState : State
    {
        private SpotOversight spotOversight;

        public BeginState(BattleSystem battleSystem, OperationManager operationManager, ChatManager chatManager,
            UIManager uiManager, PlayerManager playerManager) : base(battleSystem, operationManager, chatManager,
            uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            BattleStarter battleStarter = null;

            while (battleStarter == null)
            {
                battleStarter = this.battleSystem.GetStarter();
                yield return null;
            }

            this.battleSystem.SetSelectionMenu(this.uiManager.GetSelectionMenu());
            this.battleSystem.SetDisplayManager(this.uiManager.GetDisplayManager());

            #region Setup Spots

            foreach (BattleMember battleMember in battleStarter.GetAllBattleMembers())
            {
                battleMember.Setup();
                battleMember.GetTeam().Setup();
            }

            this.spotOversight = this.battleSystem.GetSpotOversight();

            BattleMember[] playerWithAllies = { this.playerManager.GetBattleMember() };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            playerWithAllies.Concat(battleStarter.GetAllies());
            this.spotOversight.SetupSpots(playerWithAllies, this.battleSystem.GetBattlefield().GetAllyField());
            this.spotOversight.SetupSpots(battleStarter.GetEnemies(),
                this.battleSystem.GetBattlefield().GetEnemyField());

            this.spotOversight.Reorganise(false);

            #endregion

            this.battleSystem.SetupAbilityOversight();

            #region Start Log

            string playersMsg = "Starting Battle Between:";
            string alliesMsg = " - " + this.playerManager.GetBattleMember().GetName(), enemiesMsg = "";

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in this.battleSystem.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (!battleMember || battleMember == this.playerManager.GetBattleMember()) continue;

                if (battleMember.GetTeamAffiliation() == this.playerManager.GetBattleMember().GetTeamAffiliation())
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

            while (!this.chatManager.GetIsClear())
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

                SwitchAction action = this.battleSystem.InstantiateSwitchAction();

                action.SetNextPokemon(battleMember.GetTeam().GetFirstOut());
                action.SetSpot(spot);

                OperationsContainer container = new();
                container.Add(action);
                switchInActions.Add(container);
            }

            this.operationManager.AddOperationsContainer(switchInActions.ToArray());

            #endregion

            foreach (IOperation i in switchInActions
                         .SelectMany(switchInAction =>
                             switchInAction.GetInterfaces()))
            {
                while (!i.IsOperationDone)
                    yield return null;
            }


            while (!this.chatManager.GetIsClear() || !this.operationManager.GetDone())
                yield return null;

            this.spotOversight.Reorganise(false);

            this.battleSystem.SetState(new PlayerTurnState(this.battleSystem, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
        }
    }
}