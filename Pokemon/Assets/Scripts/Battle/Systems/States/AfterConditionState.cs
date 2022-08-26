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
            this.oversight = manager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (ConditionOversight conditionOversight in this.oversight.GetSpots()
                         .Where(s => s.GetActivePokemon() != null)
                         .Select(s =>
                             s.GetActivePokemon().GetConditionOversight()))
            {
                this.manager.StartCoroutine(conditionOversight.CheckConditionEndTurn());

                while (!conditionOversight.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;

                conditionOversight.Reset();
            }

            if (this.oversight.GetSpots().FirstOrDefault(spot =>
                    spot.GetActivePokemon() == null &&
                    spot.GetBattleMember().GetTeam().CanSendMorePokemon()))
            {
                this.manager.SetState(new PlayerSelectNewState(this.manager));
                yield break;
            }

            this.manager.SetState(new RoundDoneState(manager));
        }
    }
}