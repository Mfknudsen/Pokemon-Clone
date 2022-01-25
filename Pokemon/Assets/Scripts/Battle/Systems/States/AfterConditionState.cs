#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon.Conditions;

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
                .Select(s => s.GetActivePokemon())
                .Where(p => p != null)
                .Select(p => p.GetConditionOversight()))
            {
                manager.StartCoroutine(conditionOversight.CheckConditionEndTurn());

                while (!conditionOversight.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;

                conditionOversight.Reset();
            }

            manager.SetState(new PlayerSelectNewState(manager));
        }
    }
}