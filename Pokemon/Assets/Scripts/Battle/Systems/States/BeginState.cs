#region Packages

using System.Collections;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using Mfknudsen.UI;
using UnityEngine;

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
            UIManager uiManager = UIManager.Instance;
            uiManager.SwitchUI(UISelection.Battle);

            manager.SetSelectionMenu(uiManager.GetSelectionMenu());
            manager.SetDisplayManager(uiManager.GetDisplayManager());

            #region Setup Spots

            BattleStarter battleStarter = null;

            while (battleStarter is null)
            {
                battleStarter = manager.GetStarter();
                yield return null;
            }

            foreach (BattleMember battleMember in battleStarter.GetAllBattleMembers())
            {
                battleMember.GetTeam().Setup();
                battleMember.Setup();
            }

            spotOversight = manager.GetSpotOversight();

            int offset = 0;
            //Player
            PlayerManager playerManager = PlayerManager.Instance;

            Team team = playerManager.GetTeam();

            for (int i = 0; i < battleStarter.GetPlayerSpotCount(); i++)
            {
                if (!team.CanSendMorePokemon())
                {
                    playerManager.GetBattleMember().ForceHasAllSpots();
                    break;
                }

                Spot spot = manager.CreateSpot().GetComponent<Spot>();
                spot.SetBattleMember(PlayerManager.Instance.GetBattleMember());

                offset += 1;
                spotOversight.SetSpot(spot);
                spot.transform.position = new Vector3(0 + (10 * i), 0, -10);

                playerManager.GetBattleMember().SetOwnedSpot(spot);
            }

            //Allies
            foreach (BattleMember battleMember in battleStarter.GetAllies())
            {
                team = battleMember.GetTeam();

                for (int i = 0; i < battleMember.GetSpotsToOwn(); i++)
                {
                    if (!team.CanSendMorePokemon())
                    {
                        battleMember.ForceHasAllSpots();
                        break;
                    }

                    Spot spot = manager.CreateSpot().GetComponent<Spot>();

                    spot.SetBattleMember(battleMember);

                    spotOversight.SetSpot(spot);
                    spot.transform.position = new Vector3(0 + (10 * (i + offset)), 0, -10);
                    offset += 1;
                    battleMember.SetOwnedSpot(spot);
                }
            }

            offset = 0;

            //Enemies
            foreach (BattleMember battleMember in battleStarter.GetEnemies())
            {
                team = battleMember.GetTeam();

                for (int i = 0; i < battleMember.GetSpotsToOwn(); i++)
                {
                    if (!team.CanSendMorePokemon())
                    {
                        battleMember.ForceHasAllSpots();
                        break;
                    }

                    Spot spot = manager.CreateSpot().GetComponent<Spot>();

                    spot.SetBattleMember(battleMember);

                    spotOversight.SetSpot(spot);
                    spot.transform.position = new Vector3(0 + (10 * (i + offset)), 0, 10);
                    offset += 1;
                    battleMember.SetOwnedSpot(spot);
                }
            }

            spotOversight.Reorganise(false);

            #endregion

            manager.GetDisplayManager().Setup();
            manager.GetSelectionMenu().Setup();
            manager.SetupAbilityOversight();

            #region Start Log

            string playersMsg = "Starting Battle Between:";
            string alliesMsg = " - " + playerManager.GetBattleMember().GetName(), enemiesMsg = "";

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in manager.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (battleMember is null || battleMember == playerManager.GetBattleMember()) continue;

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

            while (BattleLog.instance is null)
                yield return null;

            BattleLog.AddLog("Begin State", playersMsg);

            #endregion

            while (!ChatManager.instance.GetIsClear())
                yield return null;

            #region Start Actions

            OperationManager operationManager = OperationManager.Instance;
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
                operationManager.AddOperationsContainer(container);
            }

            #endregion

            while (!ChatManager.instance.GetIsClear() || !operationManager.GetDone())
                yield return null;

            spotOversight.Reorganise(true);

            manager.SetState(new PlayerTurnState(manager));
        }
    }
}