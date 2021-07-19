#region SDK

using System.Collections;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class AfterConditionState : State
    {
        private readonly SpotOversight oversight;

        public AfterConditionState(BattleMaster master) : base(master)
        {
            oversight = master.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (Spot spot in oversight.GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();

                if (pokemon is null) continue;
                
                ConditionOversight conditionOversight = pokemon.GetConditionOversight();

                master.StartCoroutine(conditionOversight.CheckConditionEndTurn());

                while (!conditionOversight.GetDone() || !ChatMaster.instance.GetIsClear())
                    yield return null;
                
                conditionOversight.Reset();
            }

            master.SetState(new PlayerSelectNewState(master));
            yield break;
        }
    }
}