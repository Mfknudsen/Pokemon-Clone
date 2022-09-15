#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Pokémon.Conditions;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class AfterConditionState : State
    {
        private readonly SpotOversight oversight;

        public AfterConditionState(BattleManager battleManager) : base(battleManager)
        {
            this.oversight = battleManager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (ConditionOversight conditionOversight in this.oversight.GetSpots()
                         .Where(s => s.GetActivePokemon() != null)
                         .Select(s =>
                             s.GetActivePokemon().GetConditionOversight()))
            {
                this.battleManager.StartCoroutine(conditionOversight.CheckConditionEndTurn());

                while (!conditionOversight.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;

                conditionOversight.Reset();
            }

            if (this.oversight.GetSpots().FirstOrDefault(spot =>
                    spot.GetActivePokemon() == null &&
                    spot.GetBattleMember().GetTeam().CanSendMorePokemon()))
            {
                this.battleManager.SetState(new PlayerSelectNewState(this.battleManager));
                yield break;
            }

            this.battleManager.SetState(new RoundDoneState(battleManager));
        }
    }
}