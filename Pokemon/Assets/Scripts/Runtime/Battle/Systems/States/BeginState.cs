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
using Runtime.UI;
using UnityEngine;
using Logger = Runtime._Debug.Logger;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class BeginState : State
    {
        private SpotOversight spotOversight;

        public BeginState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            UIManager uiManager = UIManager.instance;
            BattleStarter battleStarter = null;
            PlayerManager playerManager = PlayerManager.instance;

            while (battleStarter == null)
            {
                battleStarter = this.manager.GetStarter();
                yield return null;
            }

            this.manager.SetSelectionMenu(uiManager.GetSelectionMenu());
            this.manager.SetDisplayManager(uiManager.GetDisplayManager());

            #region Setup Spots

            foreach (BattleMember battleMember in battleStarter.GetAllBattleMembers())
            {
                battleMember.Setup();
                battleMember.GetTeam().Setup();
            }

            this.spotOversight = this.manager.GetSpotOversight();

            BattleMember[] playerWithAllies = { playerManager.GetBattleMember() };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            playerWithAllies.Concat(battleStarter.GetAllies());
            this.spotOversight.SetupSpots(playerWithAllies, this.manager.GetBattlefield().GetAllyField());
            this.spotOversight.SetupSpots(battleStarter.GetEnemies(), this.manager.GetBattlefield().GetEnemyField());

            this.spotOversight.Reorganise(false);

            #endregion

            this.manager.SetupAbilityOversight();

            #region Start Log

            string playersMsg = "Starting Battle Between:";
            string alliesMsg = " - " + playerManager.GetBattleMember().GetName(), enemiesMsg = "";

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in this.manager.GetSpotOversight().GetSpots())
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

            while (!ChatManager.instance.GetIsClear())
                yield return null;

            #region Start Actions

            OperationManager operationManager = OperationManager.instance;
            List<OperationsContainer> switchInActions = new();
            foreach (Spot spot in this.spotOversight.GetSpots())
            {
                spot.SetTransform();

                if (spot.GetBattleMember() is null)
                    continue;

                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = battleMember.GetTeam().GetFirstOut();

                if (!pokemon) continue;

                SwitchAction action = this.manager.InstantiateSwitchAction();

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
                while (!i.Done())
                    yield return null;
            }


            while (!ChatManager.instance.GetIsClear() || !operationManager.GetDone())
                yield return null;

            this.spotOversight.Reorganise(false);

            this.manager.SetState(new PlayerTurnState(this.manager));
        }
    }
}