#region SDK

using System.Collections;
using System.Linq;
using Mfknudsen.Player;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class RoundDoneState : State
    {
        private readonly Team playerTeam;

        public RoundDoneState(BattleMaster master) : base(master)
        {
            playerTeam = MasterPlayer.instance.GetTeam();
        }

        public override IEnumerator Tick()
        {
            SpotOversight spotOversight = master.GetSpotOversight();
            bool endBattle = true;

            // ReSharper disable once Unity.NoNullPropagation
            foreach (Spot spot in spotOversight.GetSpots().Where(spot => !(spot?.GetActivePokemon() is null) && spot.GetActivePokemon().GetTurnDone()))
                spot.GetActivePokemon().SetTurnDone(false);

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