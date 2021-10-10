#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class AfterConditionState : State
    {
        private readonly SpotOversight oversight;

        public AfterConditionState(BattleManager manager) : base(manager)
        {
            oversight = manager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (Spot spot in oversight.GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();

                if (pokemon is null) continue;
                
                ConditionOversight conditionOversight = pokemon.GetConditionOversight();

                manager.StartCoroutine(conditionOversight.CheckConditionEndTurn());

                while (!conditionOversight.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;
                
                conditionOversight.Reset();
            }

            manager.SetState(new PlayerSelectNewState(manager));
            yield break;
        }
    }
}