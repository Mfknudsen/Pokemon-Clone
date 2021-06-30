using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Comunication;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEngine;

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

            while (battleStarter == null)
            {
                battleStarter = master.GetStarter();
                yield return 0;
            }

            int allyX = battleStarter.GetPlayerSpotCount() + battleStarter.GetAllySpotCount(),
                enemyX = battleStarter.GetEnemiesSpotCount();

            spotOversight = master.SetupSpotOversight(allyX >= enemyX ? allyX : enemyX);

            int offset = 0;
            //Player
            Team team = MasterPlayer.instance.GetTeam();

            for (int i = 0; i < battleStarter.GetPlayerSpotCount(); i++)
            {
                Pokemon pokemon = team.GetPokemonByIndex(i);
                if (pokemon == null)
                {
                    MasterPlayer.instance.GetBattleMember().ForceHasAllSpots();
                    break;
                }

                Spot spot = master.CreateSpot().GetComponent<Spot>();

                spot.SetBattleMember(MasterPlayer.instance.GetBattleMember());

                offset += 1;
                spotOversight.SetSpot(spot, i, 0);
                spot.transform.position = new Vector3(0 + (10 * i), 0, -10);

                MasterPlayer.instance.GetBattleMember().SetOwndSpot(spot);
            }

            //Allies
            foreach (BattleMember battleMember in battleStarter.GetAllies())
            {
                team = battleMember.GetTeam();

                for (int i = 0; i < battleMember.GetSpotsToOwn(); i++)
                {
                    Pokemon pokemon = team.GetPokemonByIndex(i);
                    if (pokemon == null)
                    {
                        battleMember.ForceHasAllSpots();
                        break;
                    }

                    Spot spot = master.CreateSpot().GetComponent<Spot>();

                    spot.SetBattleMember(battleMember);

                    spotOversight.SetSpot(spot, i + offset, 0);
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
                    Pokemon pokemon = team.GetPokemonByIndex(i);
                    if (pokemon == null)
                    {
                        battleMember.ForceHasAllSpots();
                        break;
                    }

                    Spot spot = master.CreateSpot().GetComponent<Spot>();

                    spot.SetBattleMember(battleMember);

                    spotOversight.SetSpot(spot, i + offset, 1);
                    spot.transform.position = new Vector3(0 + (10 * (i + offset)), 0, 10);
                    offset += 1;
                    battleMember.SetOwndSpot(spot);
                }
            }

            #endregion

            #region Start Actions

            List<BattleAction> list = new List<BattleAction>();

            foreach (Spot spot in spotOversight.GetSpots())
            {
                spot.SetTransform();

                if (spot == null || spot.GetBattleMember() == null)
                    continue;

                BattleMember battleMember = spot.GetBattleMember();

                SwitchAction action = master.InstantiateSwitchAction();

                action.SetNextPokemon(battleMember.GetTeam().GetFirstOut());
                action.SetSpot(spot);

                list.Add(action);
            }

            while (!ChatMaster.instance.GetIsClear())
                yield return 0;

            while (list.Count > 0)
            {
                BattleAction action = list[0];

                master.StartCoroutine(action.Activate());

                while (!action.GetDone())
                    yield return 0;
                
                list.RemoveAt(0);
            }

            #endregion

            while (!ChatMaster.instance.GetIsClear())
                yield return 0;

            master.SetState(new PlayerTurnState(master));
        }
    }
}