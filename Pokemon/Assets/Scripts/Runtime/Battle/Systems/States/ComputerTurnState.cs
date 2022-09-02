#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Spots;
using Runtime.Pokémon;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class ComputerTurnState : State
    {
        public ComputerTurnState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            SpotOversight spotOversight = this.manager.GetSpotOversight();

            foreach (Spot spot in spotOversight.GetSpots().Where(spot =>
                spot.GetActivePokemon() != null &&
                !spot.GetBattleMember().IsPlayer()))
            {
                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = spot.GetActivePokemon();

                battleMember.ActivateAIBrain(pokemon);

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            this.manager.SetState(new ActionState(this.manager));
        }
    }
}