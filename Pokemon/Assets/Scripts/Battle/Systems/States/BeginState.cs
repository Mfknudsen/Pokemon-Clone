#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.UI;

#endregion

namespace Mfknudsen.Battle.Systems.States
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
                battleStarter = manager.GetStarter();
                yield return null;
            }

            manager.SetSelectionMenu(uiManager.GetSelectionMenu());
            manager.SetDisplayManager(uiManager.GetDisplayManager());

            #region Setup Spots

            foreach (BattleMember battleMember in battleStarter.GetAllBattleMembers())
            {
                battleMember.Setup();
                battleMember.GetTeam().Setup();
            }

            spotOversight = manager.GetSpotOversight();

            BattleMember[] playerWithAllies = { playerManager.GetBattleMember() };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            playerWithAllies.Concat(battleStarter.GetAllies());
            spotOversight.SetupSpots(playerWithAllies, manager.GetBattlefield().GetAllyField());
            spotOversight.SetupSpots(battleStarter.GetEnemies(), manager.GetBattlefield().GetEnemyField());

            spotOversight.Reorganise(false);

            #endregion

            manager.SetupAbilityOversight();

            #region Start Log

            string playersMsg = "Starting Battle Between:";
            string alliesMsg = " - " + playerManager.GetBattleMember().GetName(), enemiesMsg = "";

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in manager.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (battleMember == null || battleMember == playerManager.GetBattleMember()) continue;

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

            while (Logger.instance == null)
                yield return null;

            Logger.AddLog("Begin State", playersMsg);

            #endregion

            while (!ChatManager.instance.GetIsClear())
                yield return null;

            #region Start Actions

            OperationManager operationManager = OperationManager.instance;
            List<OperationsContainer> switchInActions = new List<OperationsContainer>();
            foreach (Spot spot in spotOversight.GetSpots())
            {
                spot.SetTransform();

                if (spot.GetBattleMember() is null)
                    continue;

                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = battleMember.GetTeam().GetFirstOut();

                if (pokemon == null) continue;

                SwitchAction action = manager.InstantiateSwitchAction();

                action.SetNextPokemon(battleMember.GetTeam().GetFirstOut());
                action.SetSpot(spot);

                OperationsContainer container = new OperationsContainer();
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

            spotOversight.Reorganise(false);

            manager.SetState(new PlayerTurnState(manager));
        }
    }
}