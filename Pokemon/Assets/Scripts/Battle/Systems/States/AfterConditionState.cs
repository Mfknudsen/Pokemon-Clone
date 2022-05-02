#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
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
            foreach (ConditionOversight conditionOversight in oversight.GetSpots()
                         .Where(s => s.GetActivePokemon() != null)
                         .Select(s =>
                             s.GetActivePokemon().GetConditionOversight()))
            {
                manager.StartCoroutine(conditionOversight.CheckConditionEndTurn());

                while (!conditionOversight.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;

                conditionOversight.Reset();
            }

            if (oversight.GetSpots().FirstOrDefault(spot =>
                    spot.GetActivePokemon() == null &&
                    spot.GetBattleMember().GetTeam().CanSendMorePokemon()))
            {
                manager.SetState(new PlayerSelectNewState(manager));
                yield break;
            }

            manager.SetState(new RoundDoneState(manager));
        }
    }
}