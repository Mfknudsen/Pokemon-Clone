using System.Collections;
using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.Battle.Systems.States
{
    public class ComputerTurnState : State
    {
        public ComputerTurnState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            yield return 0;
            Debug.Log("Start");

            SpotOversight spotOversight = master.GetSpotOversight();
            
            foreach (Spot spot in spotOversight.GetSpots())
            {
                if (spot is null)
                    continue;
                
                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = spot.GetActivePokemon();
                
                if(battleMember is null || pokemon is null || !battleMember.IsPlayer())
                    continue;

                battleMember.ActivateAIBrain();

                while (pokemon.GetBattleAction() == null)
                    yield return 0;
            }
            
            Debug.Log("Done");
        }
    }
}