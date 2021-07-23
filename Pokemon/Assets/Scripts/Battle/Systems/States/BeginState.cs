#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Comunication;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class BeginState : State
    {
        private SpotOversight spotOversight;

        public BeginState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            #region Setup Spots

            BattleStarter battleStarter = null;

            while (battleStarter is null)
            {
                battleStarter = master.GetStarter();
                yield return 0;
            }

            spotOversight = master.SetupSpotOversight();

            int offset = 0;
            //Player
            MasterPlayer player = MasterPlayer.instance;

            Team team = player.GetTeam();

            for (int i = 0; i < battleStarter.GetPlayerSpotCount(); i++)
            {
                if (!team.CanSendMorePokemon())
                {
                    player.GetBattleMember().ForceHasAllSpots();
                    break;
                }

                Spot spot = master.CreateSpot().GetComponent<Spot>();
                spot.SetBattleMember(MasterPlayer.instance.GetBattleMember());

                offset += 1;
                spotOversight.SetSpot(spot);
                spot.transform.position = new Vector3(0 + (10 * i), 0, -10);

                player.GetBattleMember().SetOwndSpot(spot);
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

                    Spot spot = master.CreateSpot().GetComponent<Spot>();

                    spot.SetBattleMember(battleMember);

                    spotOversight.SetSpot(spot);
                    spot.transform.position = new Vector3(0 + (10 * (i + offset)), 0, -10);
                    offset += 1;
                    battleMember.SetOwndSpot(spot);
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

                    Spot spot = master.CreateSpot().GetComponent<Spot>();

                    spot.SetBattleMember(battleMember);

                    spotOversight.SetSpot(spot);
                    spot.transform.position = new Vector3(0 + (10 * (i + offset)), 0, 10);
                    offset += 1;
                    battleMember.SetOwndSpot(spot);
                }
            }

            spotOversight.Reorganise(false);

            #endregion

            master.GetDisplayManager().Setup();
            master.GetSelectionMenu().Setup();
            master.SetupAbilityOversight();

            #region Start Log

            string playersMsg = "Starting Battle Between:";
            string alliesMsg = " - " + player.GetBattleMember().GetName(), enemiesMsg = "";

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in master.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (battleMember is null || battleMember == player.GetBattleMember()) continue;

                if (battleMember.GetTeamNumber() == player.GetBattleMember().GetTeamNumber())
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

            #region Start Actions

            List<BattleAction> list = new List<BattleAction>();

            foreach (Spot spot in spotOversight.GetSpots())
            {
                spot.SetTransform();

                if (spot.GetBattleMember() is null)
                    continue;

                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = battleMember.GetTeam().GetFirstOut();
                
                if(pokemon is null) continue;
                
                SwitchAction action = master.InstantiateSwitchAction();

                action.SetNextPokemon(battleMember.GetTeam().GetFirstOut());
                action.SetSpot(spot);

                list.Add(action);
            }

            while (!ChatMaster.instance.GetIsClear())
                yield return 0;

            foreach (BattleAction action in list)
            {
                master.StartCoroutine(action.Activate());

                while (!action.GetDone())
                    yield return null;
            }

            #endregion

            while (!ChatMaster.instance.GetIsClear())
                yield return 0;

            spotOversight.Reorganise(true);
            
            master.SetState(new PlayerTurnState(master));
        }
    }
}