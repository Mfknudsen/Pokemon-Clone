#region SDK

using System.Collections;
using System.Linq;
using Mfknudsen.Player;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class RoundDoneState : State
    {
        public RoundDoneState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            SpotOversight spotOversight = master.GetSpotOversight();
            bool endBattle = true;

            spotOversight.Reorganise(true);

            if (spotOversight.GetSpots().Count >= 2)
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
                bool playerVictory = MasterPlayer.instance.GetTeam().HasMorePokemon();
                Debug.Log("Victory: " + playerVictory);
                if (playerVictory)
                    master.SetState(new WinState(master));
                else
                    master.SetState(new LostState(master));
            }
            else
                master.SetState(new PlayerTurnState(master));

            yield break;
        }
    }
}